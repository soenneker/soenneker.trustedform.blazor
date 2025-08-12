export class TrustedFormInterop {
    constructor() {
        this.instances = {};
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
            tf.onload = () => resolve();
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
            fieldId: configuration.field
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
}

window.TrustedFormInterop = new TrustedFormInterop();
