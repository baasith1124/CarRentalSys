/**
 * Dashboard 3D Full - Complete Three.js Integration
 * Provides comprehensive 3D interactive dashboard experience
 */

class Dashboard3D {
    constructor() {
        this.scene = null;
        this.camera = null;
        this.renderer = null;
        this.particles = [];
        this.particleSystem = null;
        this.cardMeshes = [];
        this.mouse = { x: 0, y: 0 };
        this.windowHalf = { x: window.innerWidth / 2, y: window.innerHeight / 2 };
        this.clock = new THREE.Clock();
        this.isWebGLSupported = this.checkWebGLSupport();
        this.performanceLevel = 'high';
        
        this.init();
    }

    checkWebGLSupport() {
        try {
            const canvas = document.createElement('canvas');
            return !!(window.WebGLRenderingContext && 
                (canvas.getContext('webgl') || canvas.getContext('experimental-webgl')));
        } catch (e) {
            return false;
        }
    }

    init() {
        if (this.isWebGLSupported) {
            this.initThreeJS();
            this.createParticleSystem();
            this.create3DCards();
            this.setupEventListeners();
            this.animate();
        } else {
            this.fallbackToCSS3D();
        }
        
        this.initInteractiveElements();
        this.initPerformanceMonitoring();
    }

    initThreeJS() {
        // Scene setup
        this.scene = new THREE.Scene();
        this.scene.fog = new THREE.Fog(0x0a0a0a, 1, 3000);

        // Camera setup
        this.camera = new THREE.PerspectiveCamera(
            75, 
            window.innerWidth / window.innerHeight, 
            1, 
            3000
        );
        this.camera.position.z = 1000;

        // Renderer setup
        this.renderer = new THREE.WebGLRenderer({ 
            alpha: true, 
            antialias: true 
        });
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        this.renderer.setClearColor(0x000000, 0);
        
        // Insert canvas as background
        const canvas = this.renderer.domElement;
        canvas.style.position = 'fixed';
        canvas.style.top = '0';
        canvas.style.left = '0';
        canvas.style.zIndex = '-1';
        canvas.style.pointerEvents = 'none';
        document.body.appendChild(canvas);
    }

    createParticleSystem() {
        const particleCount = this.getParticleCount();
        const geometry = new THREE.BufferGeometry();
        const positions = new Float32Array(particleCount * 3);
        const colors = new Float32Array(particleCount * 3);
        const sizes = new Float32Array(particleCount);

        const color = new THREE.Color();

        for (let i = 0; i < particleCount; i++) {
            const i3 = i * 3;
            
            // Position
            positions[i3] = (Math.random() - 0.5) * 4000;
            positions[i3 + 1] = (Math.random() - 0.5) * 4000;
            positions[i3 + 2] = (Math.random() - 0.5) * 4000;
            
            // Color gradient
            color.setHSL(0.6 + Math.random() * 0.2, 0.7, 0.5 + Math.random() * 0.3);
            colors[i3] = color.r;
            colors[i3 + 1] = color.g;
            colors[i3 + 2] = color.b;
            
            // Size
            sizes[i] = Math.random() * 3 + 1;
        }

        geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
        geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3));
        geometry.setAttribute('size', new THREE.BufferAttribute(sizes, 1));

        const material = new THREE.ShaderMaterial({
            uniforms: {
                time: { value: 0 }
            },
            vertexShader: `
                attribute float size;
                attribute vec3 color;
                varying vec3 vColor;
                uniform float time;
                
                void main() {
                    vColor = color;
                    vec4 mvPosition = modelViewMatrix * vec4(position, 1.0);
                    gl_PointSize = size * (300.0 / -mvPosition.z) * (1.0 + 0.2 * sin(time + position.x * 0.01));
                    gl_Position = projectionMatrix * mvPosition;
                }
            `,
            fragmentShader: `
                varying vec3 vColor;
                
                void main() {
                    float r = distance(gl_PointCoord, vec2(0.5, 0.5));
                    if (r > 0.5) discard;
                    float alpha = 1.0 - smoothstep(0.0, 0.5, r);
                    gl_FragColor = vec4(vColor, alpha * 0.8);
                }
            `,
            transparent: true,
            blending: THREE.AdditiveBlending
        });

        this.particleSystem = new THREE.Points(geometry, material);
        this.scene.add(this.particleSystem);
    }

    getParticleCount() {
        switch (this.performanceLevel) {
            case 'low': return 200;
            case 'medium': return 500;
            case 'high': return 1000;
            default: return 500;
        }
    }

    create3DCards() {
        const cards = document.querySelectorAll('.stats-card, .chart-card, .recent-activity');
        
        cards.forEach((card, index) => {
            const rect = card.getBoundingClientRect();
            const geometry = new THREE.PlaneGeometry(rect.width / 4, rect.height / 4);
            const material = new THREE.MeshBasicMaterial({ 
                color: 0x667eea, 
                transparent: true, 
                opacity: 0.1 
            });
            
            const mesh = new THREE.Mesh(geometry, material);
            mesh.position.set(
                (rect.left - this.windowHalf.x) / 2,
                -(rect.top - this.windowHalf.y) / 2,
                50 + index * 10
            );
            
            mesh.userData = { 
                card: card, 
                originalPosition: mesh.position.clone(),
                index: index
            };
            
            this.cardMeshes.push(mesh);
            this.scene.add(mesh);
        });
    }

    setupEventListeners() {
        // Mouse movement
        document.addEventListener('mousemove', (event) => {
            this.mouse.x = (event.clientX - this.windowHalf.x) / this.windowHalf.x;
            this.mouse.y = -(event.clientY - this.windowHalf.y) / this.windowHalf.y;
        });

        // Window resize
        window.addEventListener('resize', () => {
            this.onWindowResize();
        });

        // Card hover effects
        document.querySelectorAll('.stats-card, .chart-card').forEach((card, index) => {
            card.addEventListener('mouseenter', () => {
                this.animateCardHover(index, true);
            });
            
            card.addEventListener('mouseleave', () => {
                this.animateCardHover(index, false);
            });
        });
    }

    animateCardHover(cardIndex, isHover) {
        if (this.cardMeshes[cardIndex]) {
            const mesh = this.cardMeshes[cardIndex];
            const targetZ = isHover ? 100 : mesh.userData.originalPosition.z;
            
            gsap.to(mesh.position, {
                z: targetZ,
                duration: 0.3,
                ease: "power2.out"
            });
            
            gsap.to(mesh.rotation, {
                x: isHover ? 0.1 : 0,
                y: isHover ? 0.1 : 0,
                duration: 0.3,
                ease: "power2.out"
            });
        }
    }

    animate() {
        if (!this.renderer) return;
        
        requestAnimationFrame(() => this.animate());
        
        const time = this.clock.getElapsedTime();
        
        // Update particle system
        if (this.particleSystem) {
            this.particleSystem.material.uniforms.time.value = time;
            this.particleSystem.rotation.y = time * 0.05;
            
            // Mouse parallax effect
            this.particleSystem.rotation.x += (this.mouse.y * 0.05 - this.particleSystem.rotation.x) * 0.05;
            this.particleSystem.rotation.y += (this.mouse.x * 0.05 - this.particleSystem.rotation.y) * 0.05;
        }

        // Update camera with mouse parallax
        this.camera.position.x += (this.mouse.x * 100 - this.camera.position.x) * 0.05;
        this.camera.position.y += (this.mouse.y * 100 - this.camera.position.y) * 0.05;
        this.camera.lookAt(this.scene.position);

        // Update card meshes
        this.cardMeshes.forEach((mesh, index) => {
            mesh.rotation.z = Math.sin(time + index) * 0.02;
        });

        this.renderer.render(this.scene, this.camera);
    }

    onWindowResize() {
        this.windowHalf.x = window.innerWidth / 2;
        this.windowHalf.y = window.innerHeight / 2;
        
        this.camera.aspect = window.innerWidth / window.innerHeight;
        this.camera.updateProjectionMatrix();
        
        this.renderer.setSize(window.innerWidth, window.innerHeight);
    }

    fallbackToCSS3D() {
        console.log('WebGL not supported, using CSS 3D fallback');
        document.body.classList.add('css3d-fallback');
        
        // Enhanced CSS 3D effects
        this.initCSS3DEffects();
    }

    initCSS3DEffects() {
        // Create floating particles with CSS
        this.createCSSParticles();
        
        // Enhanced card transforms
        const cards = document.querySelectorAll('.stats-card, .chart-card');
        cards.forEach((card, index) => {
            card.style.transform = `translateZ(${index * 10}px)`;
            card.style.transition = 'transform 0.3s ease';
        });
    }

    createCSSParticles() {
        const particleContainer = document.createElement('div');
        particleContainer.className = 'css-particles';
        particleContainer.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            pointer-events: none;
            z-index: -1;
        `;
        
        for (let i = 0; i < 50; i++) {
            const particle = document.createElement('div');
            particle.className = 'css-particle';
            particle.style.cssText = `
                position: absolute;
                width: ${Math.random() * 4 + 2}px;
                height: ${Math.random() * 4 + 2}px;
                background: radial-gradient(circle, rgba(102,126,234,0.8) 0%, transparent 70%);
                border-radius: 50%;
                left: ${Math.random() * 100}%;
                top: ${Math.random() * 100}%;
                animation: cssParticleFloat ${Math.random() * 20 + 10}s infinite linear;
            `;
            particleContainer.appendChild(particle);
        }
        
        document.body.appendChild(particleContainer);
        
        // Add CSS animation
        if (!document.getElementById('css-particle-styles')) {
            const style = document.createElement('style');
            style.id = 'css-particle-styles';
            style.textContent = `
                @keyframes cssParticleFloat {
                    0% { transform: translateY(100vh) rotate(0deg); opacity: 0; }
                    10% { opacity: 1; }
                    90% { opacity: 1; }
                    100% { transform: translateY(-100vh) rotate(360deg); opacity: 0; }
                }
            `;
            document.head.appendChild(style);
        }
    }

    initInteractiveElements() {
        // 3D Tilt effect for cards
        const cards = document.querySelectorAll('.stats-card, .chart-card, .recent-activity');
        
        cards.forEach(card => {
            card.addEventListener('mousemove', (e) => {
                const rect = card.getBoundingClientRect();
                const x = e.clientX - rect.left;
                const y = e.clientY - rect.top;
                
                const centerX = rect.width / 2;
                const centerY = rect.height / 2;
                
                const rotateX = (y - centerY) / centerY * -10;
                const rotateY = (x - centerX) / centerX * 10;
                
                card.style.transform = `
                    perspective(1000px) 
                    rotateX(${rotateX}deg) 
                    rotateY(${rotateY}deg) 
                    translateZ(20px)
                `;
            });
            
            card.addEventListener('mouseleave', () => {
                card.style.transform = 'perspective(1000px) rotateX(0) rotateY(0) translateZ(0)';
            });
        });

        // Interactive number counters with 3D effect
        this.animateCounters();
        
        // 3D Chart enhancements
        this.enhance3DCharts();
    }

    animateCounters() {
        const counters = document.querySelectorAll('.stat-number');
        
        counters.forEach(counter => {
            const target = parseInt(counter.textContent.replace(/[^\d]/g, ''));
            let current = 0;
            const increment = target / 100;
            
            const timer = setInterval(() => {
                current += increment;
                if (current >= target) {
                    current = target;
                    clearInterval(timer);
                }
                
                counter.textContent = Math.floor(current).toLocaleString();
                
                // 3D pulse effect
                counter.style.transform = `scale(${1 + Math.sin(current / target * Math.PI) * 0.1})`;
            }, 20);
        });
    }

    enhance3DCharts() {
        // Add 3D depth to existing charts
        const chartContainers = document.querySelectorAll('.chart-container');
        
        chartContainers.forEach(container => {
            container.style.transformStyle = 'preserve-3d';
            container.style.transform = 'translateZ(30px)';
        });
    }

    initPerformanceMonitoring() {
        let frameCount = 0;
        let lastTime = performance.now();
        
        const checkPerformance = () => {
            frameCount++;
            const currentTime = performance.now();
            
            if (currentTime - lastTime >= 1000) {
                const fps = frameCount;
                frameCount = 0;
                lastTime = currentTime;
                
                // Adjust quality based on FPS
                if (fps < 30 && this.performanceLevel !== 'low') {
                    this.adjustPerformance('low');
                } else if (fps > 50 && this.performanceLevel !== 'high') {
                    this.adjustPerformance('high');
                }
            }
            
            requestAnimationFrame(checkPerformance);
        };
        
        checkPerformance();
    }

    adjustPerformance(level) {
        this.performanceLevel = level;
        
        if (this.particleSystem) {
            // Recreate particle system with new count
            this.scene.remove(this.particleSystem);
            this.createParticleSystem();
        }
        
        console.log(`Performance adjusted to: ${level}`);
    }

    // Public methods
    destroy() {
        if (this.renderer) {
            document.body.removeChild(this.renderer.domElement);
            this.renderer.dispose();
        }
        
        // Remove CSS particles
        const cssParticles = document.querySelector('.css-particles');
        if (cssParticles) {
            cssParticles.remove();
        }
    }

    updateStats(newData) {
        // Update dashboard with new data and animate changes
        Object.keys(newData).forEach(key => {
            const element = document.querySelector(`[data-stat="${key}"]`);
            if (element) {
                const oldValue = parseInt(element.textContent.replace(/[^\d]/g, ''));
                const newValue = newData[key];
                
                // Animate counter
                gsap.to({ value: oldValue }, {
                    value: newValue,
                    duration: 1,
                    ease: "power2.out",
                    onUpdate: function() {
                        element.textContent = Math.floor(this.targets()[0].value).toLocaleString();
                    }
                });
                
                // 3D highlight effect
                gsap.to(element, {
                    scale: 1.2,
                    duration: 0.2,
                    yoyo: true,
                    repeat: 1,
                    ease: "power2.out"
                });
            }
        });
    }
}

// Initialize Dashboard 3D when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    // Wait for other scripts to load
    setTimeout(() => {
        window.dashboard3D = new Dashboard3D();
        console.log('Dashboard 3D initialized');
    }, 500);
});

// Real-time data simulation
setInterval(() => {
    if (window.dashboard3D) {
        const mockData = {
            totalCars: Math.floor(Math.random() * 50) + 150,
            activeRentals: Math.floor(Math.random() * 30) + 45,
            revenue: Math.floor(Math.random() * 5000) + 25000,
            customers: Math.floor(Math.random() * 20) + 320
        };
        
        window.dashboard3D.updateStats(mockData);
    }
}, 30000); // Update every 30 seconds