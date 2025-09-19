/**
 * Advanced Three.js WebGL Particle System with Physics
 * Featuring car particles, physics simulations, and interactive effects
 */

class ThreeJsWebGLSystem {
    constructor() {
        this.scene = null;
        this.camera = null;
        this.renderer = null;
        this.world = null; // Cannon.js physics world
        this.particles = [];
        this.carModels = [];
        this.mouse = { x: 0, y: 0, z: 0 };
        this.clock = new THREE.Clock();
        this.raycaster = new THREE.Raycaster();
        this.mouseVector = new THREE.Vector2();
        
        this.config = {
            particleCount: 150,
            carModelCount: 8,
            physicsEnabled: true,
            interactiveMode: true,
            colorScheme: {
                primary: new THREE.Color(0x667eea),
                secondary: new THREE.Color(0xf093fb),
                accent: new THREE.Color(0x4facfe),
                background: new THREE.Color(0x0a0a0f)
            }
        };
        
        this.init();
    }
    
    init() {
        // Check WebGL support first
        if (!this.hasWebGLSupport()) {
            console.error('WebGL not supported');
            this.showWebGLError();
            return;
        }
        
        try {
            this.createScene();
            this.createCamera();
            this.createRenderer();
            this.setupPhysicsWorld();
            this.createParticleSystem();
            this.createCarModels();
            this.setupLighting();
            this.setupEventListeners();
            this.startRenderLoop();
            
            console.log('ThreeJS WebGL System initialized successfully');
        } catch (error) {
            console.error('Failed to initialize 3D system:', error);
            this.showWebGLError();
        }
    }
    
    hasWebGLSupport() {
        try {
            const canvas = document.createElement('canvas');
            const context = canvas.getContext('webgl2') || 
                          canvas.getContext('webgl') || 
                          canvas.getContext('experimental-webgl');
            
            if (!context) {
                return false;
            }
            
            // Test basic WebGL functionality
            const gl = context;
            const buffer = gl.createBuffer();
            if (!buffer) {
                return false;
            }
            
            gl.deleteBuffer(buffer);
            return true;
        } catch (e) {
            return false;
        }
    }
    
    showWebGLError() {
        const errorDiv = document.createElement('div');
        errorDiv.id = 'webgl-error';
        errorDiv.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: rgba(255, 82, 82, 0.9);
            color: white;
            padding: 15px;
            border-radius: 8px;
            z-index: 10000;
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.2);
            max-width: 300px;
        `;
        errorDiv.innerHTML = `
            <div style="font-weight: bold; margin-bottom: 8px;">⚠️ 3D Features Unavailable</div>
            <div style="font-size: 14px; margin-bottom: 10px;">Your browser doesn't support WebGL or 3D acceleration is disabled.</div>
            <div style="font-size: 12px; opacity: 0.8;">Try updating your browser or enabling hardware acceleration.</div>
            <button onclick="this.parentElement.remove()" style="margin-top: 10px; background: rgba(255,255,255,0.2); border: none; color: white; padding: 5px 10px; border-radius: 4px; cursor: pointer;">Dismiss</button>
        `;
        document.body.appendChild(errorDiv);
        
        // Auto-remove after 10 seconds
        setTimeout(() => {
            if (errorDiv.parentElement) {
                errorDiv.remove();
            }
        }, 10000);
    }
    
    createScene() {
        this.scene = new THREE.Scene();
        this.scene.background = this.config.colorScheme.background;
        this.scene.fog = new THREE.FogExp2(0x0a0a0f, 0.002);
    }
    
    createCamera() {
        this.camera = new THREE.PerspectiveCamera(
            75,
            window.innerWidth / window.innerHeight,
            0.1,
            1000
        );
        this.camera.position.set(0, 0, 20);
    }
    
    createRenderer() {
        this.renderer = new THREE.WebGLRenderer({
            canvas: this.getOrCreateCanvas(),
            antialias: true,
            alpha: true,
            powerPreference: "high-performance"
        });
        
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
        this.renderer.shadowMap.enabled = true;
        this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;
        this.renderer.outputEncoding = THREE.sRGBEncoding;
        this.renderer.toneMapping = THREE.ACESFilmicToneMapping;
        this.renderer.toneMappingExposure = 1.2;
    }
    
    getOrCreateCanvas() {
        let canvas = document.getElementById('threejs-webgl-canvas');
        if (!canvas) {
            canvas = document.createElement('canvas');
            canvas.id = 'threejs-webgl-canvas';
            canvas.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                z-index: 1;
                pointer-events: none;
            `;
            document.body.appendChild(canvas);
        }
        return canvas;
    }
    
    setupPhysicsWorld() {
        if (!this.config.physicsEnabled) return;
        
        this.world = new CANNON.World();
        this.world.gravity.set(0, -9.82, 0);
        this.world.broadphase = new CANNON.NaiveBroadphase();
        this.world.solver.iterations = 10;
        
        // Ground plane
        const groundShape = new CANNON.Plane();
        const groundBody = new CANNON.Body({ mass: 0 });
        groundBody.addShape(groundShape);
        groundBody.quaternion.setFromAxisAngle(new CANNON.Vec3(1, 0, 0), -Math.PI / 2);
        groundBody.position.set(0, -10, 0);
        this.world.add(groundBody);
    }
    
    createParticleSystem() {
        const particleGeometry = new THREE.BufferGeometry();
        const particleCount = this.config.particleCount;
        
        const positions = new Float32Array(particleCount * 3);
        const colors = new Float32Array(particleCount * 3);
        const sizes = new Float32Array(particleCount);
        const velocities = new Float32Array(particleCount * 3);
        
        for (let i = 0; i < particleCount; i++) {
            const i3 = i * 3;
            
            // Positions in 3D space
            positions[i3] = (Math.random() - 0.5) * 100;
            positions[i3 + 1] = (Math.random() - 0.5) * 50;
            positions[i3 + 2] = (Math.random() - 0.5) * 50;
            
            // Random colors from our scheme
            const colorChoice = Math.random();
            let color;
            if (colorChoice < 0.33) {
                color = this.config.colorScheme.primary;
            } else if (colorChoice < 0.66) {
                color = this.config.colorScheme.secondary;
            } else {
                color = this.config.colorScheme.accent;
            }
            
            colors[i3] = color.r;
            colors[i3 + 1] = color.g;
            colors[i3 + 2] = color.b;
            
            // Particle sizes
            sizes[i] = Math.random() * 3 + 1;
            
            // Velocities
            velocities[i3] = (Math.random() - 0.5) * 0.02;
            velocities[i3 + 1] = (Math.random() - 0.5) * 0.02;
            velocities[i3 + 2] = (Math.random() - 0.5) * 0.02;
            
            // Create physics body for each particle
            if (this.config.physicsEnabled) {
                const shape = new CANNON.Sphere(sizes[i] * 0.1);
                const body = new CANNON.Body({ mass: 0.1 });
                body.addShape(shape);
                body.position.set(positions[i3], positions[i3 + 1], positions[i3 + 2]);
                body.velocity.set(velocities[i3], velocities[i3 + 1], velocities[i3 + 2]);
                this.world.add(body);
                this.particles.push({ body, index: i });
            }
        }
        
        particleGeometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
        particleGeometry.setAttribute('color', new THREE.BufferAttribute(colors, 3));
        particleGeometry.setAttribute('size', new THREE.BufferAttribute(sizes, 1));
        
        // Custom particle shader
        const particleMaterial = new THREE.ShaderMaterial({
            uniforms: {
                time: { value: 0 },
                mousePosition: { value: new THREE.Vector3() },
                resolution: { value: new THREE.Vector2(window.innerWidth, window.innerHeight) }
            },
            vertexShader: `
                attribute float size;
                attribute vec3 color;
                varying vec3 vColor;
                varying float vSize;
                uniform float time;
                uniform vec3 mousePosition;
                
                void main() {
                    vColor = color;
                    vSize = size;
                    
                    vec3 pos = position;
                    
                    // Mouse interaction
                    float distanceToMouse = distance(pos, mousePosition);
                    float mouseInfluence = smoothstep(10.0, 0.0, distanceToMouse);
                    pos += normalize(pos - mousePosition) * mouseInfluence * 2.0;
                    
                    // Floating animation
                    pos.y += sin(time * 0.5 + position.x * 0.1) * 0.5;
                    pos.x += cos(time * 0.3 + position.z * 0.1) * 0.3;
                    
                    vec4 mvPosition = modelViewMatrix * vec4(pos, 1.0);
                    gl_Position = projectionMatrix * mvPosition;
                    gl_PointSize = size * (300.0 / -mvPosition.z);
                }
            `,
            fragmentShader: `
                varying vec3 vColor;
                varying float vSize;
                uniform float time;
                
                void main() {
                    vec2 uv = gl_PointCoord - 0.5;
                    float dist = length(uv);
                    
                    if (dist > 0.5) discard;
                    
                    float alpha = 1.0 - smoothstep(0.0, 0.5, dist);
                    alpha *= (0.5 + 0.5 * sin(time * 2.0 + vSize));
                    
                    gl_FragColor = vec4(vColor, alpha);
                }
            `,
            transparent: true,
            vertexColors: true,
            blending: THREE.AdditiveBlending,
            depthWrite: false
        });
        
        this.particleSystem = new THREE.Points(particleGeometry, particleMaterial);
        this.scene.add(this.particleSystem);
    }
    
    createCarModels() {
        const carGeometry = new THREE.BoxGeometry(2, 0.8, 1);
        const wheelGeometry = new THREE.CylinderGeometry(0.3, 0.3, 0.2, 8);
        
        for (let i = 0; i < this.config.carModelCount; i++) {
            const carGroup = new THREE.Group();
            
            // Car body with gradient material
            const carMaterial = new THREE.MeshPhongMaterial({
                color: this.getRandomCarColor(),
                shininess: 100,
                transparent: true,
                opacity: 0.8
            });
            
            const carBody = new THREE.Mesh(carGeometry, carMaterial);
            carGroup.add(carBody);
            
            // Wheels
            const wheelMaterial = new THREE.MeshPhongMaterial({
                color: 0x333333,
                shininess: 50
            });
            
            const wheelPositions = [
                [-0.8, -0.4, 0.4],
                [0.8, -0.4, 0.4],
                [-0.8, -0.4, -0.4],
                [0.8, -0.4, -0.4]
            ];
            
            wheelPositions.forEach(pos => {
                const wheel = new THREE.Mesh(wheelGeometry, wheelMaterial);
                wheel.position.set(pos[0], pos[1], pos[2]);
                wheel.rotation.z = Math.PI / 2;
                carGroup.add(wheel);
            });
            
            // Position cars in 3D space
            carGroup.position.set(
                (Math.random() - 0.5) * 60,
                (Math.random() - 0.5) * 20,
                (Math.random() - 0.5) * 30
            );
            
            carGroup.rotation.y = Math.random() * Math.PI * 2;
            
            // Add physics body for car
            if (this.config.physicsEnabled) {
                const carShape = new CANNON.Box(new CANNON.Vec3(1, 0.4, 0.5));
                const carBody = new CANNON.Body({ mass: 5 });
                carBody.addShape(carShape);
                carBody.position.copy(carGroup.position);
                this.world.add(carBody);
                
                this.carModels.push({
                    mesh: carGroup,
                    body: carBody,
                    rotationSpeed: (Math.random() - 0.5) * 0.02
                });
            } else {
                this.carModels.push({
                    mesh: carGroup,
                    rotationSpeed: (Math.random() - 0.5) * 0.02
                });
            }
            
            this.scene.add(carGroup);
        }
    }
    
    getRandomCarColor() {
        const colors = [0x667eea, 0xf093fb, 0x4facfe, 0x84fab0, 0xff9a56];
        return colors[Math.floor(Math.random() * colors.length)];
    }
    
    setupLighting() {
        // Ambient light
        const ambientLight = new THREE.AmbientLight(0x404040, 0.6);
        this.scene.add(ambientLight);
        
        // Directional light with shadows
        const directionalLight = new THREE.DirectionalLight(0xffffff, 1);
        directionalLight.position.set(50, 50, 25);
        directionalLight.castShadow = true;
        directionalLight.shadow.mapSize.width = 2048;
        directionalLight.shadow.mapSize.height = 2048;
        directionalLight.shadow.camera.near = 0.5;
        directionalLight.shadow.camera.far = 100;
        directionalLight.shadow.camera.left = -50;
        directionalLight.shadow.camera.right = 50;
        directionalLight.shadow.camera.top = 50;
        directionalLight.shadow.camera.bottom = -50;
        this.scene.add(directionalLight);
        
        // Point lights for atmospheric effect
        const pointLight1 = new THREE.PointLight(0x667eea, 0.8, 30);
        pointLight1.position.set(-20, 10, 10);
        this.scene.add(pointLight1);
        
        const pointLight2 = new THREE.PointLight(0xf093fb, 0.8, 30);
        pointLight2.position.set(20, 10, -10);
        this.scene.add(pointLight2);
        
        const pointLight3 = new THREE.PointLight(0x4facfe, 0.6, 25);
        pointLight3.position.set(0, -10, 15);
        this.scene.add(pointLight3);
    }
    
    setupEventListeners() {
        // Mouse movement
        document.addEventListener('mousemove', (event) => {
            this.mouseVector.x = (event.clientX / window.innerWidth) * 2 - 1;
            this.mouseVector.y = -(event.clientY / window.innerHeight) * 2 + 1;
            
            // Update 3D mouse position
            this.raycaster.setFromCamera(this.mouseVector, this.camera);
            const intersects = this.raycaster.ray.origin.clone();
            intersects.add(this.raycaster.ray.direction.clone().multiplyScalar(20));
            
            this.mouse.x = intersects.x;
            this.mouse.y = intersects.y;
            this.mouse.z = intersects.z;
        });
        
        // Window resize
        window.addEventListener('resize', () => {
            this.camera.aspect = window.innerWidth / window.innerHeight;
            this.camera.updateProjectionMatrix();
            this.renderer.setSize(window.innerWidth, window.innerHeight);
        });
        
        // Mouse click interactions
        document.addEventListener('click', (event) => {
            this.handleMouseClick(event);
        });
    }
    
    handleMouseClick(event) {
        this.raycaster.setFromCamera(this.mouseVector, this.camera);
        const intersects = this.raycaster.intersectObjects(this.scene.children, true);
        
        if (intersects.length > 0) {
            const clickedObject = intersects[0].object;
            
            // Add ripple effect at click position
            this.createRippleEffect(intersects[0].point);
            
            // Add impulse to nearby physics objects
            if (this.config.physicsEnabled) {
                this.particles.forEach(particle => {
                    const distance = particle.body.position.distanceTo(intersects[0].point);
                    if (distance < 5) {
                        const force = new CANNON.Vec3();
                        force.copy(particle.body.position);
                        force.vsub(intersects[0].point);
                        force.normalize();
                        force.scale(10);
                        particle.body.applyImpulse(force, particle.body.position);
                    }
                });
            }
        }
    }
    
    createRippleEffect(position) {
        const rippleGeometry = new THREE.RingGeometry(0.1, 2, 16);
        const rippleMaterial = new THREE.MeshBasicMaterial({
            color: 0x667eea,
            transparent: true,
            opacity: 0.7,
            side: THREE.DoubleSide
        });
        
        const ripple = new THREE.Mesh(rippleGeometry, rippleMaterial);
        ripple.position.copy(position);
        ripple.lookAt(this.camera.position);
        this.scene.add(ripple);
        
        // Animate ripple with GSAP
        gsap.to(ripple.scale, {
            duration: 1,
            x: 5,
            y: 5,
            z: 5,
            ease: "power2.out"
        });
        
        gsap.to(ripple.material, {
            duration: 1,
            opacity: 0,
            ease: "power2.out",
            onComplete: () => {
                this.scene.remove(ripple);
                rippleGeometry.dispose();
                rippleMaterial.dispose();
            }
        });
    }
    
    startRenderLoop() {
        const animate = () => {
            requestAnimationFrame(animate);
            this.update();
            this.render();
        };
        animate();
    }
    
    update() {
        const deltaTime = this.clock.getDelta();
        const elapsedTime = this.clock.getElapsedTime();
        
        // Update physics world
        if (this.config.physicsEnabled && this.world) {
            this.world.step(deltaTime);
            
            // Sync particle positions with physics
            this.particles.forEach(particle => {
                const positions = this.particleSystem.geometry.attributes.position.array;
                const index = particle.index * 3;
                positions[index] = particle.body.position.x;
                positions[index + 1] = particle.body.position.y;
                positions[index + 2] = particle.body.position.z;
            });
            this.particleSystem.geometry.attributes.position.needsUpdate = true;
            
            // Sync car positions with physics
            this.carModels.forEach(car => {
                if (car.body) {
                    car.mesh.position.copy(car.body.position);
                    car.mesh.quaternion.copy(car.body.quaternion);
                }
            });
        }
        
        // Update particle shader uniforms
        if (this.particleSystem.material.uniforms) {
            this.particleSystem.material.uniforms.time.value = elapsedTime;
            this.particleSystem.material.uniforms.mousePosition.value.set(
                this.mouse.x, this.mouse.y, this.mouse.z
            );
        }
        
        // Rotate cars smoothly
        this.carModels.forEach(car => {
            if (!this.config.physicsEnabled) {
                car.mesh.rotation.y += car.rotationSpeed;
                car.mesh.position.y += Math.sin(elapsedTime + car.mesh.position.x * 0.1) * 0.01;
            }
        });
        
        // Camera gentle movement
        this.camera.position.x += (this.mouse.x * 0.01 - this.camera.position.x) * 0.02;
        this.camera.position.y += (this.mouse.y * 0.01 - this.camera.position.y) * 0.02;
        this.camera.lookAt(this.scene.position);
    }
    
    render() {
        if (this.isContextLost) return;
        this.renderer.render(this.scene, this.camera);
    }
    
    // Public methods for external control
    setParticleCount(count) {
        this.config.particleCount = count;
        // Recreate particle system with new count
        this.scene.remove(this.particleSystem);
        this.createParticleSystem();
    }
    
    togglePhysics(enabled) {
        this.config.physicsEnabled = enabled;
        if (enabled && !this.world) {
            this.setupPhysicsWorld();
        }
    }
    
    dispose() {
        // Clean up resources
        if (this.renderer) {
            this.renderer.dispose();
        }
        if (this.particleSystem) {
            this.particleSystem.geometry.dispose();
            this.particleSystem.material.dispose();
        }
        this.carModels.forEach(car => {
            car.mesh.geometry.dispose();
            car.mesh.material.dispose();
        });
    }
}

// Auto-initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    if (typeof THREE !== 'undefined' && typeof CANNON !== 'undefined') {
        window.threeJsWebGLSystem = new ThreeJsWebGLSystem();
    } else {
        console.warn('Three.js or Cannon.js not loaded. Skipping WebGL system initialization.');
    }
});