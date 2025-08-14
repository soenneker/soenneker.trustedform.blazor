export class TrustedFormInterop {
    constructor() {
        this.instances = {};
    }

    // Helper function to log debug messages only when debug is enabled
    debugLog(configuration, message, ...args) {
        if (configuration.debug) {
            console.log(message, ...args);
        }
    }

    async loadTrustedFormScript(configuration) {
        // Remove any existing TrustedForm scripts
        const existingScripts = document.querySelectorAll('script[src*="api.trustedform.com/trustedform.js"]');
        existingScripts.forEach(script => script.remove());

        // Clean up TrustedForm global variables
        if (window.trustedForm) {
            delete window.trustedForm;
        }

        // Build query string from configuration
        const params = [
            `field=${encodeURIComponent(configuration.field)}`,
            `invert_field_sensitivity=${configuration.invertFieldSensitivity ? 'true' : 'false'}`,
            `sandbox=${configuration.sandbox ? 'true' : 'false'}`,
            `use_tagged_consent=${configuration.useTaggedConsent ? 'true' : 'false'}`
        ];

        if (configuration.disableRecording === true) {
            params.push('disable_recording=true');
        }

        const src =
            (window.location.protocol === 'https:' ? 'https' : 'http') +
            '://api.trustedform.com/trustedform.js?' +
            params.join('&') +
            '&l=' + (new Date().getTime() + Math.random());

        // Load the TrustedForm script
        return new Promise((resolve, reject) => {
            const tf = document.createElement('script');
            tf.type = 'text/javascript';
            tf.async = true;
            tf.src = src;
            tf.onload = () => {
                resolve();
            };
            tf.onerror = () => reject(new Error('Failed to load TrustedForm script'));
            
            const s = document.getElementsByTagName('script')[0];
            s.parentNode.insertBefore(tf, s);
        });
    }

    async init(elementId, configuration, dotNetCallback) {
        if (this.instances[elementId]?.isLoaded)
            return;

        // Load the TrustedForm script for this configuration
        await this.loadTrustedFormScript(configuration);

        this.instances[elementId] = {
            dotNetCallback,
            isLoaded: true,
            fieldId: configuration.field,
            configuration: configuration // Store configuration for debug access
        };

        // Call the callback after script is loaded
        if (dotNetCallback) {
            dotNetCallback.invokeMethodAsync('OnLoadCallback');
        }
    }

    createObserver(elementId) {
        const target = document.getElementById(elementId);
        if (!target || !target.parentNode)
            return null;

        const observer = new MutationObserver((mutations) => {
            const removed = mutations.some(m =>
                Array.from(m.removedNodes).includes(target)
            );
            if (removed) {
                // Clean up the instance and its hidden field
                this.removeInstance(elementId);
            }
        });

        observer.observe(target.parentNode, { childList: true });

        if (this.instances[elementId]) {
            this.instances[elementId].observer = observer;
        }

        return observer;
    }

    getCertUrl(elementId) {
        // Try to find the cert field for this instance
        const instance = this.instances[elementId];

        if (!instance)
            return null;

        // Get the field for this specific instance
        const fieldId = instance.fieldId;
        const input = document.getElementById(fieldId + "_0");

        if (input && input.value)
            return input.value;

        if (input && input.textContent)
            return input.textContent;

        return null;
    }

    getCertUrlForSingleElement() {
        const instanceKeys = Object.keys(this.instances);
        
        // If there's exactly one instance, return its cert URL
        if (instanceKeys.length === 1) {
            const elementId = instanceKeys[0];
            return this.getCertUrl(elementId);
        }
        
        // Return null if there are multiple instances or no instances
        return null;
    }

    start() {
        window.trustedFormStartRecording();
    }

    stop() {
        window.trustedFormStopRecording();
    }

    finalize(elementId, configuration) {
        // TrustedForm finalization is triggered by form submission
        // Since IncludeForm=true creates a form element, we trigger finalization
        // by submitting that existing form
        if (window.trustedForm && window.trustedForm.id) {
            // Find the configuration from any instance to access debug flag
            const instanceKeys = Object.keys(this.instances);
            const config = configuration || (instanceKeys.length > 0 ? this.instances[instanceKeys[0]].configuration : { debug: false });
            
            this.debugLog(config, 'TrustedForm is available, ID:', window.trustedForm.id);
            this.debugLog(config, 'Number of instances:', instanceKeys.length);
            
            // Find the form element directly by ID
            let targetForm = null;
            for (const id in this.instances) {
                const formId = id + "-form";
                const formElement = document.getElementById(formId);
                this.debugLog(config, 'Looking for form with ID:', formId, 'found:', formElement);
                if (formElement) {
                    targetForm = formElement;
                    this.debugLog(config, 'Found form element:', targetForm);
                    break;
                }
            }
            
            if (targetForm) {
                // If IncludeForm is true, prevent the form from actually submitting
                if (config.includeForm === true) {
                    // Add a temporary event listener to prevent form submission
                    const preventSubmit = (e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        this.debugLog(config, 'Form submission prevented during finalization');
                        targetForm.removeEventListener('submit', preventSubmit);
                    };
                    
                    targetForm.addEventListener('submit', preventSubmit);
                }
                
                // Try different approaches to trigger finalization
                this.debugLog(config, 'Attempting to trigger TrustedForm finalization...');
                
                const submitButton = targetForm.querySelector('button[type="submit"]');
                if (submitButton) {
                    submitButton.click();
                    this.debugLog(config, 'Method 1: Native form submission via submit button click');
                } else {
                    // Fallback: Use form.submit() method
                    targetForm.submit();
                    this.debugLog(config, 'Method 1: Native form submission via form.submit()');
                }
                                
                this.debugLog(config, 'TrustedForm finalization attempts completed');
            } else {
                this.debugLog(config, 'No TrustedForm-generated form found. Make sure IncludeForm=true is set in configuration.');
                // Additional debugging: check if forms exist in DOM
                const allForms = document.querySelectorAll('form');
                this.debugLog(config, 'All forms in DOM:', allForms.length);
                allForms.forEach((form, index) => {
                    this.debugLog(config, `Form ${index}:`, form);
                });
            }
        } else {
            this.debugLog(config, 'TrustedForm not available or not initialized. Make sure TrustedForm is loaded and initialized.');
        }
    }
}

window.TrustedFormInterop = new TrustedFormInterop();
