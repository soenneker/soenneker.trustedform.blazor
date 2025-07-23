export class TrustedFormInterop {
    constructor() {
        this.instances = {};
    }

    init(elementId, configuration, dotNetCallback) {
        if (this.instances[elementId]?.isLoaded)
            return;

        // Build query string from configuration
        const params = [
            `field=${encodeURIComponent(configuration.Field ?? 'xxTrustedFormCertUrl')}`,
            `invert_field_sensitivity=${configuration.InvertFieldSensitivity ? 'true' : 'false'}`,
            `sandbox=${configuration.Sandbox ? 'true' : 'false'}`,
            `use_tagged_consent=${configuration.UseTaggedConsent ? 'true' : 'false'}`
        ];

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
            isLoaded: true
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
}

window.TrustedFormInterop = new TrustedFormInterop();
