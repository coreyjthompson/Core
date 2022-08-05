var meiAbbViePortal;
(function (meiAbbViePortal) {
    var common;
    (function (common) {
        var keyboard = /** @class */ (function () {
            function keyboard() {
            }
            ;
            // function: prevents the return key from submitting the form
            keyboard.prototype.keyDownTextField = function (e) {
                var keyCode = e.keyCode;
                if (keyCode == 13) {
                    event.preventDefault();
                }
            };
            return keyboard;
        }());
        common.keyboard = keyboard;
    })(common = meiAbbViePortal.common || (meiAbbViePortal.common = {}));
})(meiAbbViePortal || (meiAbbViePortal = {}));
