mergeInto(LibraryManager.library, {

    IsInCache: function (str) {
        if ('serviceWorker' in navigator) {
            navigator.serviceWorker.controller.postMessage({type: 'IsInCached', value: 'example'});
        }

        return buffer;
    },
});

navigator.serviceWorker.addEventListener('message', event => {
    if (event.data && event.data.type === 'result') {
        const result = event.data.value;
        // boolの結果を使用して何かを行う
        console.log('Result:', result);
    }
});