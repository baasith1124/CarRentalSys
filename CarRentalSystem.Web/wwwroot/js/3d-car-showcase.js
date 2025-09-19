/**
 * Interactive 3D Car Showcase with Physics and Camera Movements
 * Advanced Three.js implementation for car rental system
 */

class CarShowcase3D {
    constructor() {
        this.scene = null;
        this.camera = null;
        this.renderer = null;
        this.controls = null;
        this.world = null; // Cannon.js physics world
        this.cars = [];
        this.selectedCar = null;
        this.clock = new THREE.Clock();
        this.raycaster = new THREE.Raycaster();
        this.mouse = new THREE.Vector2();
        
        this.config = {
            enablePhysics: true,
            autoRotate: true,
            showroom: {
                width: 40,
                height: 20,
                depth: 30
            },
            lighting: {
                ambient: 0.4,
                directional: 0.8,
                point: 0.6
            }
        };
        
        this.init();
    }
    
    init() {
        try {
            // Check WebGL support
            if (!this.hasWebGLSupport()) {
                console.error('WebGL not supported for 3D showcase');
                this.showWebGLError();
                return;
            }
            
            this.createScene();
            this.createCamera();
            this.createRenderer();
            this.setupPhysics();
            this.createShowroom();
            this.createLighting();
            this.loadCarModels();
            this.setupControls();
            this.setupEventListeners();
            this.startRenderLoop();
            
            console.log('3D Car Showcase initialized successfully');
        } catch (error) {
            console.error('Failed to initialize 3D Car Showcase:', error);
            this.showWebGLError();
        }
    }
    
    hasWebGLSupport() {
        try {
            const canvas = document.createElement('canvas');
            const context = canvas.getContext('webgl2') || 
                          canvas.getContext('webgl') || 
                          canvas.getContext('experimental-webgl');
            return !!context;
        } catch (e) {
            return false;
        }
    }
    
    showWebGLError() {
        alert('3D showcase requires WebGL support. Please try a modern browser or enable hardware acceleration.');
    }
    
    createScene() {
        this.scene = new THREE.Scene();
        this.scene.background = new THREE.Color(0x1a1a2e);
        this.scene.fog = new THREE.FogExp2(0x1a1a2e, 0.015);
    }
    
    createCamera() {
        this.camera = new THREE.PerspectiveCamera(
            60,
            window.innerWidth / window.innerHeight,
            0.1,
            200
        );
        this.camera.position.set(0, 8, 15);
        this.camera.lookAt(0, 0, 0);
    }
    
    createRenderer() {
        const canvas = this.getOrCreateShowcaseCanvas();
        this.renderer = new THREE.WebGLRenderer({
            canvas: canvas,
            antialias: true,
            alpha: true
        });
        
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
        this.renderer.shadowMap.enabled = true;
        this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;
        this.renderer.outputEncoding = THREE.sRGBEncoding;
        this.renderer.toneMapping = THREE.ACESFilmicToneMapping;
        this.renderer.toneMappingExposure = 1.0;
    }
    
    getOrCreateShowcaseCanvas() {
        let canvas = document.getElementById('car-showcase-canvas');
        if (!canvas) {
            canvas = document.createElement('canvas');
            canvas.id = 'car-showcase-canvas';
            canvas.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                z-index: 5;
                pointer-events: auto;
                opacity: 0;
                visibility: hidden;
                transition: all 0.5s ease;
            `;
            document.body.appendChild(canvas);
        }
        return canvas;
    }
    
    setupPhysics() {
        if (!this.config.enablePhysics) return;
        
        this.world = new CANNON.World();
        this.world.gravity.set(0, -20, 0);
        this.world.broadphase = new CANNON.NaiveBroadphase();
        this.world.solver.iterations = 10;
        
        // Ground
        const groundShape = new CANNON.Plane();
        const groundBody = new CANNON.Body({ mass: 0 });
        groundBody.addShape(groundShape);
        groundBody.quaternion.setFromAxisAngle(new CANNON.Vec3(1, 0, 0), -Math.PI / 2);
        this.world.add(groundBody);
    }
    
    createShowroom() {
        // Floor
        const floorGeometry = new THREE.PlaneGeometry(
            this.config.showroom.width, 
            this.config.showroom.depth
        );
        const floorMaterial = new THREE.MeshLambertMaterial({
            color: 0x2a2a3e,
            transparent: true,
            opacity: 0.8
        });
        const floor = new THREE.Mesh(floorGeometry, floorMaterial);
        floor.rotation.x = -Math.PI / 2;
        floor.receiveShadow = true;
        this.scene.add(floor);
        
        // Grid pattern on floor
        const gridHelper = new THREE.GridHelper(
            this.config.showroom.width, 
            20, 
            0x667eea, 
            0x4a4a5e
        );
        gridHelper.material.opacity = 0.3;
        gridHelper.material.transparent = true;
        this.scene.add(gridHelper);
        
        // Walls with glass effect
        this.createWalls();
        
        // Display platforms for cars
        this.createDisplayPlatforms();
    }
    
    createWalls() {
        const wallMaterial = new THREE.MeshPhysicalMaterial({
            color: 0x667eea,
            metalness: 0.1,
            roughness: 0.1,
            transparency: 0.7,
            transmission: 0.3,
            ior: 1.5
        });
        
        const wallGeometry = new THREE.PlaneGeometry(
            this.config.showroom.width, 
            this.config.showroom.height
        );
        
        // Back wall
        const backWall = new THREE.Mesh(wallGeometry, wallMaterial);
        backWall.position.set(0, this.config.showroom.height / 2, -this.config.showroom.depth / 2);
        this.scene.add(backWall);
        
        // Side walls
        const sideWallGeometry = new THREE.PlaneGeometry(
            this.config.showroom.depth, 
            this.config.showroom.height
        );
        
        const leftWall = new THREE.Mesh(sideWallGeometry, wallMaterial);
        leftWall.position.set(-this.config.showroom.width / 2, this.config.showroom.height / 2, 0);
        leftWall.rotation.y = Math.PI / 2;
        this.scene.add(leftWall);
        
        const rightWall = new THREE.Mesh(sideWallGeometry, wallMaterial);
        rightWall.position.set(this.config.showroom.width / 2, this.config.showroom.height / 2, 0);
        rightWall.rotation.y = -Math.PI / 2;
        this.scene.add(rightWall);
    }
    
    createDisplayPlatforms() {
        const platformGeometry = new THREE.CylinderGeometry(3, 3, 0.3, 16);
        const platformMaterial = new THREE.MeshPhysicalMaterial({
            color: 0x667eea,
            metalness: 0.8,
            roughness: 0.2,
            envMapIntensity: 1
        });
        
        const positions = [
            { x: -8, z: 0 },
            { x: 0, z: 0 },
            { x: 8, z: 0 },
            { x: -4, z: -8 },
            { x: 4, z: -8 }
        ];
        
        positions.forEach((pos, index) => {
            const platform = new THREE.Mesh(platformGeometry, platformMaterial);
            platform.position.set(pos.x, 0.15, pos.z);
            platform.castShadow = true;
            platform.receiveShadow = true;
            this.scene.add(platform);
            
            // Add rotating light ring around platform
            this.createPlatformLighting(pos.x, pos.z, index);
        });
    }
    
    createPlatformLighting(x, z, index) {
        const ringGeometry = new THREE.RingGeometry(3.2, 3.5, 32);
        const ringMaterial = new THREE.MeshBasicMaterial({
            color: new THREE.Color().setHSL((index * 0.2) % 1, 0.8, 0.6),
            transparent: true,
            opacity: 0.6,
            side: THREE.DoubleSide
        });
        
        const ring = new THREE.Mesh(ringGeometry, ringMaterial);
        ring.position.set(x, 0.05, z);
        ring.rotation.x = -Math.PI / 2;
        this.scene.add(ring);
        
        // Animate ring rotation
        gsap.to(ring.rotation, {
            z: Math.PI * 2,
            duration: 10 + index * 2,
            ease: "none",
            repeat: -1
        });
    }
    
    createLighting() {
        // Ambient light
        const ambientLight = new THREE.AmbientLight(0x404040, this.config.lighting.ambient);
        this.scene.add(ambientLight);
        
        // Main directional light
        const directionalLight = new THREE.DirectionalLight(0xffffff, this.config.lighting.directional);
        directionalLight.position.set(10, 20, 10);
        directionalLight.castShadow = true;
        directionalLight.shadow.mapSize.width = 2048;
        directionalLight.shadow.mapSize.height = 2048;
        directionalLight.shadow.camera.near = 0.5;
        directionalLight.shadow.camera.far = 50;
        directionalLight.shadow.camera.left = -20;
        directionalLight.shadow.camera.right = 20;
        directionalLight.shadow.camera.top = 20;
        directionalLight.shadow.camera.bottom = -20;
        this.scene.add(directionalLight);
        
        // Spotlight for dramatic effect
        const spotLight = new THREE.SpotLight(0x667eea, 1.5, 30, Math.PI / 8, 0.3);
        spotLight.position.set(0, 15, 5);
        spotLight.target.position.set(0, 0, 0);
        spotLight.castShadow = true;
        this.scene.add(spotLight);
        this.scene.add(spotLight.target);
        
        // Point lights for accent
        const colors = [0x667eea, 0xf093fb, 0x4facfe, 0x84fab0, 0xff9a56];
        colors.forEach((color, index) => {
            const pointLight = new THREE.PointLight(color, this.config.lighting.point, 15);
            const angle = (index / colors.length) * Math.PI * 2;
            pointLight.position.set(
                Math.cos(angle) * 12,
                5,
                Math.sin(angle) * 12
            );
            this.scene.add(pointLight);
        });
    }
    
    loadCarModels() {
        const carData = [
            { name: "Sports Car", color: 0xff4444, position: { x: -8, z: 0 } },
            { name: "Luxury Sedan", color: 0x4444ff, position: { x: 0, z: 0 } },
            { name: "SUV", color: 0x44ff44, position: { x: 8, z: 0 } },
            { name: "Convertible", color: 0xffff44, position: { x: -4, z: -8 } },
            { name: "Electric Car", color: 0xff44ff, position: { x: 4, z: -8 } }
        ];
        
        carData.forEach((data, index) => {
            this.createCarModel(data, index);
        });
    }
    
    createCarModel(data, index) {
        const carGroup = new THREE.Group();
        carGroup.userData = { name: data.name, index: index };
        
        // Car body
        const bodyGeometry = new THREE.BoxGeometry(4, 1.5, 2);
        const bodyMaterial = new THREE.MeshPhysicalMaterial({
            color: data.color,
            metalness: 0.9,
            roughness: 0.1,
            clearcoat: 1.0,
            clearcoatRoughness: 0.1
        });
        const body = new THREE.Mesh(bodyGeometry, bodyMaterial);
        body.position.y = 1;
        body.castShadow = true;
        carGroup.add(body);
        
        // Car roof
        const roofGeometry = new THREE.BoxGeometry(3.5, 1, 1.8);
        const roof = new THREE.Mesh(roofGeometry, bodyMaterial);
        roof.position.set(0, 2, 0);
        roof.castShadow = true;
        carGroup.add(roof);
        
        // Wheels
        const wheelGeometry = new THREE.CylinderGeometry(0.4, 0.4, 0.3, 16);
        const wheelMaterial = new THREE.MeshPhysicalMaterial({
            color: 0x333333,
            metalness: 0.8,
            roughness: 0.3
        });
        
        const wheelPositions = [
            [-1.5, 0.4, 0.8],
            [1.5, 0.4, 0.8],
            [-1.5, 0.4, -0.8],
            [1.5, 0.4, -0.8]
        ];
        
        wheelPositions.forEach(pos => {
            const wheel = new THREE.Mesh(wheelGeometry, wheelMaterial);
            wheel.position.set(pos[0], pos[1], pos[2]);
            wheel.rotation.z = Math.PI / 2;
            wheel.castShadow = true;
            carGroup.add(wheel);
        });
        
        // Position car on platform
        carGroup.position.set(data.position.x, 0.5, data.position.z);
        carGroup.rotation.y = index * 0.3; // Slight rotation for variety
        
        // Add physics body
        if (this.config.enablePhysics) {
            const carShape = new CANNON.Box(new CANNON.Vec3(2, 1, 1));
            const carBody = new CANNON.Body({ mass: 0 }); // Static for display
            carBody.addShape(carShape);
            carBody.position.copy(carGroup.position);
            this.world.add(carBody);
            
            carGroup.userData.physicsBody = carBody;
        }
        
        // Add floating animation
        gsap.to(carGroup.position, {
            y: carGroup.position.y + 0.3,
            duration: 2 + index * 0.5,
            ease: "power1.inOut",
            yoyo: true,
            repeat: -1
        });
        
        // Add subtle rotation
        gsap.to(carGroup.rotation, {
            y: carGroup.rotation.y + Math.PI * 2,
            duration: 20 + index * 5,
            ease: "none",
            repeat: -1
        });
        
        this.cars.push(carGroup);
        this.scene.add(carGroup);
    }
    
    setupControls() {
        // Check for OrbitControls availability with multiple possible locations
        let OrbitControls = null;
        
        if (typeof THREE !== 'undefined') {
            OrbitControls = THREE.OrbitControls || window.THREE?.OrbitControls;
        }
        
        // Fallback to global OrbitControls
        if (!OrbitControls && typeof window.OrbitControls !== 'undefined') {
            OrbitControls = window.OrbitControls;
        }
        
        if (OrbitControls) {
            try {
                this.controls = new OrbitControls(this.camera, this.renderer.domElement);
                this.controls.enableDamping = true;
                this.controls.dampingFactor = 0.05;
                this.controls.enableZoom = true;
                this.controls.enablePan = true;
                this.controls.autoRotate = this.config.autoRotate;
                this.controls.autoRotateSpeed = 0.5;
                this.controls.minDistance = 5;
                this.controls.maxDistance = 50;
                this.controls.maxPolarAngle = Math.PI / 2;
                console.log('OrbitControls initialized successfully');
            } catch (error) {
                console.warn('Failed to initialize OrbitControls:', error);
                this.controls = null;
            }
        } else {
            console.warn('OrbitControls not available, using manual camera controls');
            this.controls = null;
            this.setupManualControls();
        }
    }
    
    setupManualControls() {
        // Basic manual camera controls when OrbitControls is not available
        let isMouseDown = false;
        let mouseX = 0;
        let mouseY = 0;
        
        this.renderer.domElement.addEventListener('mousedown', (event) => {
            isMouseDown = true;
            mouseX = event.clientX;
            mouseY = event.clientY;
        });
        
        this.renderer.domElement.addEventListener('mouseup', () => {
            isMouseDown = false;
        });
        
        this.renderer.domElement.addEventListener('mousemove', (event) => {
            if (!isMouseDown) return;
            
            const deltaX = event.clientX - mouseX;
            const deltaY = event.clientY - mouseY;
            
            // Rotate camera around center
            const spherical = new THREE.Spherical();
            spherical.setFromVector3(this.camera.position);
            spherical.theta -= deltaX * 0.01;
            spherical.phi += deltaY * 0.01;
            spherical.phi = Math.max(0.1, Math.min(Math.PI - 0.1, spherical.phi));
            
            this.camera.position.setFromSpherical(spherical);
            this.camera.lookAt(0, 0, 0);
            
            mouseX = event.clientX;
            mouseY = event.clientY;
        });
        
        // Zoom with wheel
        this.renderer.domElement.addEventListener('wheel', (event) => {
            const distance = this.camera.position.length();
            const newDistance = Math.max(5, Math.min(50, distance + event.deltaY * 0.01));
            
            this.camera.position.normalize().multiplyScalar(newDistance);
            event.preventDefault();
        });
        
        console.log('Manual camera controls initialized');
    }
    
    setupEventListeners() {
        // Mouse interaction
        this.renderer.domElement.addEventListener('click', (event) => {
            this.onMouseClick(event);
        });
        
        this.renderer.domElement.addEventListener('mousemove', (event) => {
            this.onMouseMove(event);
        });
        
        // Window resize
        window.addEventListener('resize', () => {
            this.onWindowResize();
        });
        
        // Keyboard controls
        document.addEventListener('keydown', (event) => {
            this.onKeyDown(event);
        });
    }
    
    onMouseClick(event) {
        const rect = this.renderer.domElement.getBoundingClientRect();
        this.mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
        this.mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;
        
        this.raycaster.setFromCamera(this.mouse, this.camera);
        const intersects = this.raycaster.intersectObjects(this.cars, true);
        
        if (intersects.length > 0) {
            const selectedObject = intersects[0].object;
            const carGroup = this.findCarGroup(selectedObject);
            
            if (carGroup) {
                this.selectCar(carGroup);
            }
        }
    }
    
    onMouseMove(event) {
        const rect = this.renderer.domElement.getBoundingClientRect();
        this.mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
        this.mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;
        
        // Highlight cars on hover
        this.raycaster.setFromCamera(this.mouse, this.camera);
        const intersects = this.raycaster.intersectObjects(this.cars, true);
        
        // Reset all cars to normal
        this.cars.forEach(car => {
            car.scale.setScalar(1);
        });
        
        if (intersects.length > 0) {
            const hoveredObject = intersects[0].object;
            const carGroup = this.findCarGroup(hoveredObject);
            
            if (carGroup) {
                carGroup.scale.setScalar(1.05);
                this.renderer.domElement.style.cursor = 'pointer';
            }
        } else {
            this.renderer.domElement.style.cursor = 'default';
        }
    }
    
    findCarGroup(object) {
        let current = object;
        while (current) {
            if (this.cars.includes(current)) {
                return current;
            }
            current = current.parent;
        }
        return null;
    }
    
    selectCar(carGroup) {
        if (this.selectedCar === carGroup) return;
        
        // Reset previous selection
        if (this.selectedCar) {
            gsap.to(this.selectedCar.scale, {
                x: 1, y: 1, z: 1,
                duration: 0.3,
                ease: "back.out(1.7)"
            });
        }
        
        this.selectedCar = carGroup;
        
        // Highlight selected car
        gsap.to(carGroup.scale, {
            x: 1.2, y: 1.2, z: 1.2,
            duration: 0.5,
            ease: "back.out(1.7)"
        });
        
        // Focus camera on selected car
        this.focusOnCar(carGroup);
        
        // Show car details
        this.showCarDetails(carGroup);
        
        console.log(`Selected car: ${carGroup.userData.name}`);
    }
    
    focusOnCar(carGroup) {
        const targetPosition = carGroup.position.clone();
        targetPosition.y += 3;
        targetPosition.z += 8;
        
        gsap.to(this.camera.position, {
            x: targetPosition.x,
            y: targetPosition.y,
            z: targetPosition.z,
            duration: 2,
            ease: "power2.inOut"
        });
        
        if (this.controls) {
            gsap.to(this.controls.target, {
                x: carGroup.position.x,
                y: carGroup.position.y + 1,
                z: carGroup.position.z,
                duration: 2,
                ease: "power2.inOut"
            });
        }
    }
    
    showCarDetails(carGroup) {
        // Create or update car details overlay
        let detailsPanel = document.getElementById('car-details-panel');
        if (!detailsPanel) {
            detailsPanel = document.createElement('div');
            detailsPanel.id = 'car-details-panel';
            detailsPanel.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                width: 300px;
                background: rgba(0, 0, 0, 0.8);
                color: white;
                padding: 20px;
                border-radius: 10px;
                backdrop-filter: blur(10px);
                z-index: 1000;
                transform: translateX(100%);
                transition: transform 0.3s ease;
            `;
            document.body.appendChild(detailsPanel);
        }
        
        detailsPanel.innerHTML = `
            <h3>${carGroup.userData.name}</h3>
            <p><strong>Model:</strong> Premium ${carGroup.userData.name}</p>
            <p><strong>Price:</strong> $${50 + carGroup.userData.index * 25}/day</p>
            <p><strong>Features:</strong></p>
            <ul>
                <li>Automatic Transmission</li>
                <li>GPS Navigation</li>
                <li>Premium Sound System</li>
                <li>Climate Control</li>
            </ul>
            <button onclick="window.carShowcase3D.closeShowcase()" style="
                background: #667eea;
                color: white;
                border: none;
                padding: 10px 20px;
                border-radius: 5px;
                cursor: pointer;
                margin-top: 10px;
            ">Book Now</button>
        `;
        
        // Animate panel in
        setTimeout(() => {
            detailsPanel.style.transform = 'translateX(0)';
        }, 100);
    }
    
    onKeyDown(event) {
        switch (event.code) {
            case 'Escape':
                this.closeShowcase();
                break;
            case 'Space':
                this.config.autoRotate = !this.config.autoRotate;
                if (this.controls) {
                    this.controls.autoRotate = this.config.autoRotate;
                }
                break;
        }
    }
    
    onWindowResize() {
        this.camera.aspect = window.innerWidth / window.innerHeight;
        this.camera.updateProjectionMatrix();
        this.renderer.setSize(window.innerWidth, window.innerHeight);
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
        
        if (this.config.enablePhysics && this.world) {
            this.world.step(deltaTime);
        }
        
        if (this.controls) {
            this.controls.update();
        }
    }
    
    render() {
        this.renderer.render(this.scene, this.camera);
    }
    
    // Public methods
    show() {
        const canvas = this.renderer.domElement;
        canvas.style.opacity = '1';
        canvas.style.visibility = 'visible';
        canvas.style.pointerEvents = 'auto';
        
        // Animate camera entrance
        gsap.from(this.camera.position, {
            y: 20,
            z: 25,
            duration: 2,
            ease: "power2.out"
        });
    }
    
    hide() {
        const canvas = this.renderer.domElement;
        canvas.style.opacity = '0';
        canvas.style.visibility = 'hidden';
        canvas.style.pointerEvents = 'none';
    }
    
    closeShowcase() {
        this.hide();
        
        // Remove details panel
        const detailsPanel = document.getElementById('car-details-panel');
        if (detailsPanel) {
            detailsPanel.remove();
        }
        
        // Reset camera
        gsap.to(this.camera.position, {
            x: 0, y: 8, z: 15,
            duration: 1,
            ease: "power2.out"
        });
        
        if (this.controls) {
            gsap.to(this.controls.target, {
                x: 0, y: 0, z: 0,
                duration: 1,
                ease: "power2.out"
            });
        }
    }
    
    dispose() {
        // Clean up resources
        if (this.renderer) {
            this.renderer.dispose();
        }
        
        this.cars.forEach(car => {
            car.traverse(child => {
                if (child.geometry) child.geometry.dispose();
                if (child.material) {
                    if (Array.isArray(child.material)) {
                        child.material.forEach(material => material.dispose());
                    } else {
                        child.material.dispose();
                    }
                }
            });
        });
        
        const canvas = document.getElementById('car-showcase-canvas');
        if (canvas) {
            canvas.remove();
        }
        
        const detailsPanel = document.getElementById('car-details-panel');
        if (detailsPanel) {
            detailsPanel.remove();
        }
    }
}

// Enhanced initialization function with better error handling
function initCarShowcase() {
    try {
        // Check if all required libraries are loaded
        if (typeof THREE === 'undefined') {
            console.warn('Three.js not loaded for showcase');
            return false;
        }
        
        if (typeof CANNON === 'undefined') {
            console.warn('Cannon.js not loaded for showcase, physics will be disabled');
        }
        
        if (typeof gsap === 'undefined') {
            console.warn('GSAP not loaded for showcase, animations will be limited');
        }
        
        // Initialize the showcase
        window.carShowcase3D = new CarShowcase3D();
        console.log('3D Car Showcase initialized successfully');
        return true;
        
    } catch (error) {
        console.error('Failed to initialize 3D Car Showcase:', error);
        return false;
    }
}

// Auto-initialize when libraries are loaded
document.addEventListener('DOMContentLoaded', () => {
    // Delay initialization to ensure all libraries are loaded
    setTimeout(() => {
        if (initCarShowcase()) {
            console.log('3D Car Showcase ready');
        } else {
            console.warn('Required libraries not loaded for 3D Car Showcase');
        }
    }, 2000);
});

// Export for external use
if (typeof module !== 'undefined' && module.exports) {
    module.exports = CarShowcase3D;
}