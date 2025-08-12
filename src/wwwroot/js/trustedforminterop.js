export class TrustedFormInterop {
    constructor() {
        this.instances = {};
    }

    init(elementId, configuration, dotNetCallback) {
        if (this.instances[elementId]?.isLoaded)
            return;

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

        // Inject the TrustedForm script
        (function () {
            var tf = document.createElement('script');
            tf.type = 'text/javascript';
            tf.async = true;
            tf.src = src;
            tf.onload = function() {
                if (dotNetCallback) {
                    dotNetCallback.invokeMethodAsync('OnLoadCallback');
                }
            };
            var s = document.getElementsByTagName('script')[0];
            s.parentNode.insertBefore(tf, s);
        })();

        this.instances[elementId] = {
            dotNetCallback,
            isLoaded: true,
            fieldId: configuration.field
        };
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
                // Clean up if needed
                if (this.instances[elementId]) {
                    delete this.instances[elementId];
                }
            }
        });

        observer.observe(target.parentNode, { childList: true });

        if (this.instances[elementId]) {
            this.instances[elementId].observer = observer;
        }

        return observer;
    }

    getCertUrl(elementId) {
        // Try to find the cert field inside the element
        const instance = this.instances[elementId];

        if (!instance)
            return null;

        // Default field name
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
