mergeInto(LibraryManager.library, {
    SetCanvasResolution: function (width, height) {
        var canvas = document.getElementById("unity-canvas");
        canvas.width = width;
        canvas.height = height;
    }
});