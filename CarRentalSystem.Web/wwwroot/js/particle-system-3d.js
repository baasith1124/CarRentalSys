// Enhanced Car Particle System for 3D Effects with Anime.js Integration
class ParticleSystem {
    constructor() {
        this.particles = [];
        this.carParticles = [];
        this.maxParticles = 60;
        this.maxCarParticles = 8;
        this.canvas = null;
        this.ctx = null;
        this.animationId = null;
        this.mouseX = 0;
        this.mouseY = 0;
        this.animeAnimations = [];
        this.init();
    }

    init() {
        this.createCanvas();
        this.setupMouseTracking();
        this.generateParticles();
        this.generateCarParticles();
        this.setupAnimeIntegration();
        this.startAnimation();
    }

    createCanvas() {
        this.canvas = document.createElement('canvas');
        this.canvas.id = 'particle-canvas';
        this.canvas.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            pointer-events: none;
            z-index: 1;
            opacity: 0.6;
        `;
        
        document.body.appendChild(this.canvas);
        this.ctx = this.canvas.getContext('2d');
        this.resizeCanvas();
        
        window.addEventListener('resize', () => this.resizeCanvas());
    }

    resizeCanvas() {
        this.canvas.width = window.innerWidth;
        this.canvas.height = window.innerHeight;
    }

    setupMouseTracking() {
        document.addEventListener('mousemove', (e) => {
            this.mouseX = e.clientX;
            this.mouseY = e.clientY;
            
            // Anime.js mouse interaction effect
            if (typeof anime !== 'undefined') {
                this.createMouseRipple(e.clientX, e.clientY);
            }
        });
    }
    
    createMouseRipple(x, y) {
        // Create temporary ripple element
        const ripple = document.createElement('div');
        ripple.style.cssText = `
            position: fixed;
            left: ${x}px;
            top: ${y}px;
            width: 20px;
            height: 20px;
            background: radial-gradient(circle, rgba(102, 126, 234, 0.3), transparent);
            border-radius: 50%;
            pointer-events: none;
            z-index: 1;
            transform: translate(-50%, -50%);
        `;
        
        document.body.appendChild(ripple);
        
        // Animate ripple with Anime.js
        if (typeof anime !== 'undefined') {
            anime({
                targets: ripple,
                scale: [1, 10],
                opacity: [0.6, 0],
                duration: 800,
                easing: 'easeOutExpo',
                complete: () => {
                    document.body.removeChild(ripple);
                }
            });
        }
    }

    generateParticles() {
        for (let i = 0; i < this.maxParticles; i++) {
            this.particles.push({
                x: Math.random() * window.innerWidth,
                y: Math.random() * window.innerHeight,
                vx: (Math.random() - 0.5) * 0.8,
                vy: (Math.random() - 0.5) * 0.8,
                size: Math.random() * 4 + 1,
                opacity: Math.random() * 0.6 + 0.3,
                color: this.getRandomColor(),
                angle: Math.random() * Math.PI * 2,
                rotationSpeed: (Math.random() - 0.5) * 0.02,
                // Anime.js integration properties
                scale: 1,
                burstScale: 1,
                targetColor: null
            });
        }
    }

    generateCarParticles() {
        for (let i = 0; i < this.maxCarParticles; i++) {
            this.carParticles.push({
                x: Math.random() * window.innerWidth,
                y: Math.random() * window.innerHeight,
                vx: (Math.random() - 0.5) * 0.3,
                vy: (Math.random() - 0.5) * 0.3,
                size: Math.random() * 20 + 15,
                opacity: Math.random() * 0.4 + 0.1,
                color: this.getCarColor(),
                angle: Math.random() * Math.PI * 2,
                rotationSpeed: (Math.random() - 0.5) * 0.01,
                type: Math.floor(Math.random() * 3), // 0: car, 1: circle, 2: star
                // Anime.js integration properties
                animeRotateY: 0,
                animeScale: 1
            });
        }
    }

    getRandomColor() {
        const colors = [
            'rgba(102, 126, 234, 0.4)',
            'rgba(240, 147, 251, 0.4)',
            'rgba(79, 172, 254, 0.4)',
            'rgba(255, 255, 255, 0.3)',
            'rgba(34, 211, 238, 0.4)',
            'rgba(168, 85, 247, 0.4)'
        ];
        return colors[Math.floor(Math.random() * colors.length)];
    }

    getCarColor() {
        const colors = [
            'rgba(102, 126, 234, 0.6)',
            'rgba(240, 147, 251, 0.6)',
            'rgba(79, 172, 254, 0.6)',
            'rgba(34, 211, 238, 0.6)'
        ];
        return colors[Math.floor(Math.random() * colors.length)];
    }

    updateParticles() {
        const time = Date.now() * 0.001;
        
        // Update regular particles
        this.particles.forEach(particle => {
            // Mouse interaction
            const dx = this.mouseX - particle.x;
            const dy = this.mouseY - particle.y;
            const distance = Math.sqrt(dx * dx + dy * dy);
            
            if (distance < 100) {
                const force = (100 - distance) / 100;
                particle.vx += (dx / distance) * force * 0.01;
                particle.vy += (dy / distance) * force * 0.01;
            }
            
            particle.x += particle.vx;
            particle.y += particle.vy;
            particle.angle += particle.rotationSpeed;

            // Wrap around screen
            if (particle.x < -10) particle.x = this.canvas.width + 10;
            if (particle.x > this.canvas.width + 10) particle.x = -10;
            if (particle.y < -10) particle.y = this.canvas.height + 10;
            if (particle.y > this.canvas.height + 10) particle.y = -10;

            // Dynamic opacity
            particle.opacity = Math.abs(Math.sin(time + particle.x * 0.01)) * 0.6 + 0.2;
            
            // Damping
            particle.vx *= 0.995;
            particle.vy *= 0.995;
        });
        
        // Update car particles
        this.carParticles.forEach(particle => {
            particle.x += particle.vx;
            particle.y += particle.vy;
            particle.angle += particle.rotationSpeed;

            // Wrap around screen
            if (particle.x < -30) particle.x = this.canvas.width + 30;
            if (particle.x > this.canvas.width + 30) particle.x = -30;
            if (particle.y < -30) particle.y = this.canvas.height + 30;
            if (particle.y > this.canvas.height + 30) particle.y = -30;

            // Floating effect
            particle.y += Math.sin(time * 0.5 + particle.x * 0.01) * 0.1;
        });
    }

    drawParticles() {
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Draw connections between nearby particles
        this.drawConnections();
        
        // Draw regular particles with anime effects
        this.particles.forEach(particle => {
            this.ctx.save();
            this.ctx.translate(particle.x, particle.y);
            this.ctx.rotate(particle.angle);
            
            // Apply anime.js scale effects
            const totalScale = particle.scale * particle.burstScale;
            this.ctx.scale(totalScale, totalScale);
            
            this.ctx.beginPath();
            this.ctx.arc(0, 0, particle.size, 0, Math.PI * 2);
            
            // Use target color if transitioning
            const currentColor = particle.targetColor || particle.color;
            this.ctx.fillStyle = currentColor;
            this.ctx.globalAlpha = particle.opacity;
            
            // Enhanced glow effect
            this.ctx.shadowColor = currentColor;
            this.ctx.shadowBlur = 15 * totalScale;
            this.ctx.fill();
            this.ctx.shadowBlur = 0;
            
            this.ctx.restore();
        });
        
        // Draw car particles with anime effects
        this.carParticles.forEach(particle => {
            this.ctx.save();
            this.ctx.translate(particle.x, particle.y);
            this.ctx.rotate(particle.angle + (particle.animeRotateY * Math.PI / 180));
            this.ctx.scale(particle.animeScale, particle.animeScale);
            this.ctx.globalAlpha = particle.opacity;
            
            this.drawCarShape(particle);
            
            this.ctx.restore();
        });
        
        this.ctx.globalAlpha = 1;
    }
    
    drawConnections() {
        for (let i = 0; i < this.particles.length; i++) {
            for (let j = i + 1; j < this.particles.length; j++) {
                const dx = this.particles[i].x - this.particles[j].x;
                const dy = this.particles[i].y - this.particles[j].y;
                const distance = Math.sqrt(dx * dx + dy * dy);
                
                if (distance < 150) {
                    this.ctx.beginPath();
                    this.ctx.moveTo(this.particles[i].x, this.particles[i].y);
                    this.ctx.lineTo(this.particles[j].x, this.particles[j].y);
                    this.ctx.strokeStyle = `rgba(102, 126, 234, ${0.1 * (1 - distance / 150)})`;
                    this.ctx.lineWidth = 1;
                    this.ctx.stroke();
                }
            }
        }
    }
    
    drawCarShape(particle) {
        const size = particle.size;
        this.ctx.fillStyle = particle.color;
        this.ctx.shadowColor = particle.color;
        this.ctx.shadowBlur = 10;
        
        if (particle.type === 0) {
            // Car shape
            this.ctx.beginPath();
            this.ctx.roundRect(-size/2, -size/3, size, size/1.5, 3);
            this.ctx.fill();
            
            // Car windows
            this.ctx.fillStyle = 'rgba(255, 255, 255, 0.3)';
            this.ctx.beginPath();
            this.ctx.roundRect(-size/3, -size/4, size/1.5, size/4, 2);
            this.ctx.fill();
        } else if (particle.type === 1) {
            // Circle
            this.ctx.beginPath();
            this.ctx.arc(0, 0, size/2, 0, Math.PI * 2);
            this.ctx.fill();
        } else {
            // Star
            this.drawStar(size/2);
        }
        
        this.ctx.shadowBlur = 0;
    }
    
    drawStar(radius) {
        const spikes = 5;
        const outerRadius = radius;
        const innerRadius = radius * 0.4;
        
        this.ctx.beginPath();
        
        for (let i = 0; i < spikes * 2; i++) {
            const angle = (i * Math.PI) / spikes;
            const r = i % 2 === 0 ? outerRadius : innerRadius;
            const x = Math.cos(angle) * r;
            const y = Math.sin(angle) * r;
            
            if (i === 0) {
                this.ctx.moveTo(x, y);
            } else {
                this.ctx.lineTo(x, y);
            }
        }
        
        this.ctx.closePath();
        this.ctx.fill();
    }

    startAnimation() {
        const animate = () => {
            this.updateParticles();
            this.drawParticles();
            this.animationId = requestAnimationFrame(animate);
        };
        animate();
    }

    destroy() {
        if (this.animationId) {
            cancelAnimationFrame(this.animationId);
        }
        if (this.canvas) {
            document.body.removeChild(this.canvas);
        }
        
        // Clean up anime animations
        this.animeAnimations.forEach(animation => {
            if (animation.pause) animation.pause();
        });
    }
}

// Initialize enhanced particle system only on landing page
if (document.querySelector('.hero-section')) {
    // Wait for DOM to be fully loaded
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => {
            window.particleSystem = new ParticleSystem();
        });
    } else {
        window.particleSystem = new ParticleSystem();
    }
}