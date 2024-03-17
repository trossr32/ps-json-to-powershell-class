window.clipboardCopy = {
    copyText: function (text) {
        navigator.clipboard.writeText(text).then(function () {
            return true;
        })
            .catch(function (error) {
                return false;
            });
    }
};