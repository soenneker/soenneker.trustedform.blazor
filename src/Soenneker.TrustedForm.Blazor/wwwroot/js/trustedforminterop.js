const instances = {};

function debugLog(configuration, message, ...args) {
    if (configuration?.debug) {
        console.log(message, ...args);
    }
}

function removeTrustedFormScript() {
    const existingScripts = document.querySelectorAll('script[src*="api.trustedform.com/trustedform.js"]');

    existingScripts.forEach(script => script.remove());

    if (window.trustedForm) {
        delete window.trustedForm;
    }
}

function removeInstance(elementId) {
    const instance = instances[elementId];

    if (!instance) {
        return;
    }

    if (instance.observer) {
        instance.observer.disconnect();
    }

    delete instances[elementId];
}

async function loadTrustedFormScript(configuration) {
    removeTrustedFormScript();

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
        `${window.location.protocol === 'https:' ? 'https' : 'http'}://api.trustedform.com/trustedform.js?${params.join('&')}` +
        `&l=${new Date().getTime() + Math.random()}`;

    await new Promise((resolve, reject) => {
        const script = document.createElement('script');
        script.type = 'text/javascript';
        script.async = true;
        script.src = src;
        script.onload = resolve;
        script.onerror = () => reject(new Error('Failed to load TrustedForm script'));

        const firstScript = document.getElementsByTagName('script')[0];
        firstScript.parentNode.insertBefore(script, firstScript);
    });
}

export async function init(elementId, configuration, dotNetCallback) {
    if (instances[elementId]?.isLoaded) {
        return;
    }

    await loadTrustedFormScript(configuration);

    instances[elementId] = {
        configuration,
        dotNetCallback,
        fieldId: configuration.field,
        isLoaded: true,
        observer: null
    };

    if (dotNetCallback) {
        await dotNetCallback.invokeMethodAsync('OnLoadCallback');
    }
}

export function createObserver(elementId) {
    const target = document.getElementById(elementId);

    if (!target || !target.parentNode) {
        return null;
    }

    const observer = new MutationObserver(mutations => {
        const removed = mutations.some(mutation => Array.from(mutation.removedNodes).includes(target));

        if (removed) {
            removeInstance(elementId);
        }
    });

    observer.observe(target.parentNode, { childList: true });

    if (instances[elementId]) {
        instances[elementId].observer = observer;
    }

    return observer;
}

export function getCertUrl(elementId) {
    const instance = instances[elementId];

    if (!instance) {
        return null;
    }

    const input = document.getElementById(`${instance.fieldId}_0`);

    if (input?.value) {
        return input.value;
    }

    if (input?.textContent) {
        return input.textContent;
    }

    return null;
}

export function getCertUrlForSingleElement() {
    const instanceKeys = Object.keys(instances);

    if (instanceKeys.length !== 1) {
        return null;
    }

    return getCertUrl(instanceKeys[0]);
}

export function start() {
    window.trustedFormStartRecording();
}

export function stop() {
    window.trustedFormStopRecording();
}

export function finalize(elementId, configuration) {
    const instance = instances[elementId];
    const config = configuration || instance?.configuration || { debug: false };

    if (!window.trustedForm || !window.trustedForm.id) {
        debugLog(config, 'TrustedForm not available or not initialized.');
        return;
    }

    const form = document.getElementById(`${elementId}-form`);

    if (!form) {
        debugLog(config, `No form found (${elementId}-form).`);
        return;
    }

    const preventNavigation = event => {
        event.preventDefault();
        debugLog(config, 'Navigation prevented for TrustedForm finalization.');
    };

    form.addEventListener('submit', preventNavigation, { capture: true, once: true });

    const button = document.getElementById(`${elementId}-submit`);

    if (!button) {
        debugLog(config, 'Hidden submit button not found.');
        return;
    }

    debugLog(config, 'Clicking hidden submit button.');
    button.click();
    debugLog(config, 'Hidden submit click dispatched.');
}
