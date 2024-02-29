window.handleWindowResize = (dotNetObjRef) => {
    function onWindowResize() {
        const width = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
        const height = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
        dotNetObjRef.invokeMethodAsync('UpdateWindowSize', width, height);
    }

    // Attach the event handler
    window.addEventListener('resize', onWindowResize);

    // Call the event handler initially to set the initial window size
    onWindowResize();

    // Clean up the event handler when the component is removed
    return () => {
        window.removeEventListener('resize', onWindowResize);
    };
};