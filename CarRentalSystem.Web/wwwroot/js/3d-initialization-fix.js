/**
 * Enhanced 3D System Initialization Fix
 * This script provides better error handling and initialization for the 3D features
 */

class ThreeJSInitializationManager {
    constructor() {
        this.initializationAttempts = 0;
        this.maxAttempts = 15;
        this.retryDelay = 500;
        this.librariesLoaded = false;
        this.webglSupported = false;
        
        this.init();
    }
    
    init() {
        console.log('🚀 Starting 3D System Initialization Manager');
        this.checkWebGLSupport();
        this.waitForLibraries();
    }
    
    checkWebGLSupport() {
        try {
            const canvas = document.createElement('canvas');
            const gl = canvas.getContext('webgl2') || 
                      canvas.getContext('webgl') || 
                      canvas.getContext('experimental-webgl');
            
            if (!gl) {
                this.webglSupported = false;
                this.showWebGLError();
                return;
            }
            
            // Test basic WebGL functionality
            const buffer = gl.createBuffer();
            if (!buffer) {
                this.webglSupported = false;
                this.showWebGLError();
                return;
            }
            
            gl.deleteBuffer(buffer);
            this.webglSupported = true;
            console.log('✅ WebGL support confirmed');
            
        } catch (error) {
            this.webglSupported = false;
            this.showWebGLError();
            console.error('WebGL test failed:', error);
        }
    }
    
    showWebGLError() {
        console.error('❌ WebGL not supported');
        
        // Create error notification
        const errorDiv = document.createElement('div');
        errorDiv.id = 'webgl-error-notification';
        errorDiv.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: linear-gradient(135deg, #ff5252, #ff1744);
            color: white;
            padding: 15px 20px;
            border-radius: 8px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.3);
            z-index: 10000;
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.2);
            max-width: 350px;
            animation: slideIn 0.3s ease-out;
        `;
        
        errorDiv.innerHTML = `
            <div style="display: flex; align-items: center; margin-bottom: 8px;">
                <span style="font-size: 18px; margin-right: 8px;">⚠️</span>
                <strong>3D Features Unavailable</strong>
            </div>
            <div style="font-size: 14px; margin-bottom: 12px;">
                Your browser doesn't support WebGL or 3D acceleration is disabled.
            </div>
            <div style="font-size: 12px; opacity: 0.9; margin-bottom: 10px;">
                Try: Update browser, enable hardware acceleration, or use Chrome/Firefox.
            </div>
            <button onclick="this.parentElement.remove()" style="
                background: rgba(255,255,255,0.2);
                border: 1px solid rgba(255,255,255,0.3);
                color: white;
                padding: 6px 12px;
                border-radius: 4px;
                cursor: pointer;
                font-size: 12px;
            ">Dismiss</button>
        `;
        
        // Add animation styles
        const style = document.createElement('style');
        style.textContent = `
            @keyframes slideIn {
                from { transform: translateX(100%); opacity: 0; }
                to { transform: translateX(0); opacity: 1; }
            }
        `;
        document.head.appendChild(style);
        document.body.appendChild(errorDiv);
        
        // Auto-remove after 15 seconds
        setTimeout(() => {
            if (errorDiv.parentElement) {
                errorDiv.style.animation = 'slideIn 0.3s ease-out reverse';
                setTimeout(() => errorDiv.remove(), 300);
            }
        }, 15000);
    }
    
    waitForLibraries() {
        const checkLibraries = () => {
            this.initializationAttempts++;
            
            const threejsLoaded = typeof THREE !== 'undefined';
            const cannonLoaded = typeof CANNON !== 'undefined';
            const gsapLoaded = typeof gsap !== 'undefined';
            
            console.log(`📋 Library check attempt ${this.initializationAttempts}:`, {
                'Three.js': threejsLoaded ? '✅' : '❌',
                'Cannon.js': cannonLoaded ? '✅' : '❌',
                'GSAP': gsapLoaded ? '✅' : '❌'
            });
            
            if (threejsLoaded && cannonLoaded && gsapLoaded) {
                this.librariesLoaded = true;
                this.initializeSystems();
                return;
            }
            
            if (this.initializationAttempts < this.maxAttempts) {
                setTimeout(checkLibraries, this.retryDelay);
            } else {
                this.showLibraryError();
            }
        };
        
        checkLibraries();
    }
    
    showLibraryError() {
        console.error('❌ Required libraries failed to load after', this.maxAttempts, 'attempts');
        
        const errorDiv = document.createElement('div');
        errorDiv.style.cssText = `
            position: fixed;
            top: 20px;
            left: 50%;
            transform: translateX(-50%);
            background: linear-gradient(135deg, #ff9800, #f57c00);
            color: white;
            padding: 15px 20px;
            border-radius: 8px;
            z-index: 10000;
            backdrop-filter: blur(10px);
            max-width: 400px;
            text-align: center;
        `;
        
        errorDiv.innerHTML = `
            <div style="font-weight: bold; margin-bottom: 8px;">📚 3D Libraries Loading</div>
            <div style="font-size: 14px; margin-bottom: 10px;">
                3D features are still loading. Please wait or refresh the page.
            </div>
            <button onclick="window.location.reload()" style="
                background: rgba(255,255,255,0.2);
                border: 1px solid rgba(255,255,255,0.3);
                color: white;
                padding: 8px 16px;
                border-radius: 4px;
                cursor: pointer;
                margin-right: 10px;
            ">Refresh Page</button>
            <button onclick="this.parentElement.remove()" style="
                background: transparent;
                border: 1px solid rgba(255,255,255,0.3);
                color: white;
                padding: 8px 16px;
                border-radius: 4px;
                cursor: pointer;
            ">Continue</button>
        `;
        
        document.body.appendChild(errorDiv);
        
        setTimeout(() => {
            if (errorDiv.parentElement) {
                errorDiv.remove();
            }
        }, 10000);
    }
    
    initializeSystems() {
        if (!this.webglSupported) {
            console.warn('⚠️ Skipping 3D system initialization due to WebGL issues');
            return;
        }
        
        try {
            console.log('🎯 Initializing 3D systems...');
            
            // Initialize WebGL system
            if (typeof ThreeJsWebGLSystem !== 'undefined') {
                if (!window.threeJsWebGLSystem) {
                    window.threeJsWebGLSystem = new ThreeJsWebGLSystem();
                    console.log('✅ WebGL particle system initialized');
                }
            }
            
            // Initialize enhanced landing animations
            if (typeof ThreeJsEnhancedLanding !== 'undefined') {
                if (!window.threeJsEnhancedLanding) {
                    window.threeJsEnhancedLanding = new ThreeJsEnhancedLanding();
                    console.log('✅ Enhanced landing animations initialized');
                }
            }
            
            // Initialize car showcase (lazy loading)
            if (typeof CarShowcase3D !== 'undefined') {
                console.log('✅ Car showcase system ready for on-demand initialization');
            }
            
            this.showSuccessNotification();
            
        } catch (error) {
            console.error('❌ Failed to initialize 3D systems:', error);
            this.showInitializationError(error);
        }
    }
    
    showSuccessNotification() {
        const successDiv = document.createElement('div');
        successDiv.style.cssText = `
            position: fixed;
            bottom: 20px;
            right: 20px;
            background: linear-gradient(135deg, #4caf50, #388e3c);
            color: white;
            padding: 12px 18px;
            border-radius: 8px;
            z-index: 10000;
            backdrop-filter: blur(10px);
            animation: slideInUp 0.3s ease-out;
        `;
        
        successDiv.innerHTML = `
            <div style="display: flex; align-items: center;">
                <span style="font-size: 16px; margin-right: 8px;">🎨</span>
                <strong>3D Features Ready!</strong>
            </div>
        `;
        
        const style = document.createElement('style');
        style.textContent = `
            @keyframes slideInUp {
                from { transform: translateY(100%); opacity: 0; }
                to { transform: translateY(0); opacity: 1; }
            }
        `;
        document.head.appendChild(style);
        document.body.appendChild(successDiv);
        
        // Auto-remove after 3 seconds
        setTimeout(() => {
            if (successDiv.parentElement) {
                successDiv.style.animation = 'slideInUp 0.3s ease-out reverse';
                setTimeout(() => successDiv.remove(), 300);
            }
        }, 3000);
    }
    
    showInitializationError(error) {
        const errorDiv = document.createElement('div');
        errorDiv.style.cssText = `
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background: rgba(244, 67, 54, 0.95);
            color: white;
            padding: 20px;
            border-radius: 8px;
            z-index: 10000;
            backdrop-filter: blur(10px);
            max-width: 400px;
            text-align: center;
        `;
        
        errorDiv.innerHTML = `
            <div style="font-size: 18px; margin-bottom: 10px;">⚠️ 3D System Error</div>
            <div style="margin-bottom: 15px;">
                An error occurred while initializing 3D features.
            </div>
            <div style="font-size: 12px; margin-bottom: 15px; opacity: 0.8;">
                Error: ${error.message}
            </div>
            <button onclick="window.location.reload()" style="
                background: rgba(255,255,255,0.2);
                border: 1px solid rgba(255,255,255,0.3);
                color: white;
                padding: 10px 20px;
                border-radius: 4px;
                cursor: pointer;
            ">Reload Page</button>
        `;
        
        document.body.appendChild(errorDiv);
    }
    
    // Enhanced showcase initialization
    initializeShowcase() {
        if (!this.webglSupported) {
            alert('3D showcase requires WebGL support. Please update your browser or enable hardware acceleration.');
            return false;
        }
        
        if (!this.librariesLoaded) {
            alert('3D libraries are still loading. Please wait a moment and try again.');
            return false;
        }
        
        try {
            if (!window.carShowcase3D && typeof CarShowcase3D !== 'undefined') {
                console.log('🎬 Initializing 3D Car Showcase...');
                window.carShowcase3D = new CarShowcase3D();
            }
            
            if (window.carShowcase3D) {
                window.carShowcase3D.show();
                this.showShowcaseControls();
                return true;
            }
            
            return false;
            
        } catch (error) {
            console.error('Failed to initialize showcase:', error);
            alert('Failed to initialize 3D showcase: ' + error.message);
            return false;
        }
    }
    
    showShowcaseControls() {
        // Remove existing controls
        const existingControls = document.getElementById('showcase-controls');
        if (existingControls) {
            existingControls.remove();
        }
        
        const controls = document.createElement('div');
        controls.id = 'showcase-controls';
        controls.style.cssText = `
            position: fixed;
            top: 20px;
            left: 20px;
            z-index: 1001;
            background: rgba(0, 0, 0, 0.8);
            color: white;
            padding: 15px;
            border-radius: 8px;
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        `;
        
        controls.innerHTML = `
            <div style="margin-bottom: 10px; font-weight: bold;">🎮 3D Showcase Controls</div>
            <button onclick="if(window.carShowcase3D && window.carShowcase3D.controls) { window.carShowcase3D.config.autoRotate = !window.carShowcase3D.config.autoRotate; if(window.carShowcase3D.controls) window.carShowcase3D.controls.autoRotate = window.carShowcase3D.config.autoRotate; }" style="background: #667eea; color: white; border: none; padding: 8px 12px; border-radius: 4px; margin: 2px; cursor: pointer;">Toggle Rotation</button>
            <button onclick="if(window.carShowcase3D) { window.carShowcase3D.closeShowcase(); } document.getElementById('showcase-controls').remove();" style="background: #ff4757; color: white; border: none; padding: 8px 12px; border-radius: 4px; margin: 2px; cursor: pointer;">Close (ESC)</button>
            <div style="font-size: 12px; margin-top: 10px; opacity: 0.7;">
                🖱️ Click cars to select • Drag to rotate • Scroll to zoom
            </div>
        `;
        
        document.body.appendChild(controls);
    }
}

// Enhanced global function for showing 3D showcase
function show3DShowcase() {
    console.log('🎬 3D showcase requested');
    
    if (window.threeJSInitManager) {
        return window.threeJSInitManager.initializeShowcase();
    } else {
        console.error('3D initialization manager not ready');
        alert('3D system is still initializing. Please wait a moment and try again.');
        return false;
    }
}

// Initialize the manager when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    // Wait a bit for other scripts to load
    setTimeout(() => {
        window.threeJSInitManager = new ThreeJSInitializationManager();
    }, 100);
});

// Handle page visibility changes for performance
document.addEventListener('visibilitychange', () => {
    if (document.hidden) {
        // Pause animations when page is hidden
        if (window.threeJsWebGLSystem && typeof window.threeJsWebGLSystem.pause === 'function') {
            window.threeJsWebGLSystem.pause();
        }
        if (window.threeJsEnhancedLanding && typeof window.threeJsEnhancedLanding.pauseAllAnimations === 'function') {
            window.threeJsEnhancedLanding.pauseAllAnimations();
        }
    } else {
        // Resume animations when page is visible
        if (window.threeJsWebGLSystem && typeof window.threeJsWebGLSystem.resume === 'function') {
            window.threeJsWebGLSystem.resume();
        }
        if (window.threeJsEnhancedLanding && typeof window.threeJsEnhancedLanding.resumeAllAnimations === 'function') {
            window.threeJsEnhancedLanding.resumeAllAnimations();
        }
    }
});

console.log('🔧 3D Initialization Fix script loaded');