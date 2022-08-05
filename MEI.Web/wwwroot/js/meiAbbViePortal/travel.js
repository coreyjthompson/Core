var meiAbbViePortal;
(function (meiAbbViePortal) {
    var travel;
    (function (travel) {
        var invoice = /** @class */ (function () {
            function invoice() {
            }
            ;
            return invoice;
        }());
        travel.invoice = invoice;
    })(travel = meiAbbViePortal.travel || (meiAbbViePortal.travel = {}));
})(meiAbbViePortal || (meiAbbViePortal = {}));
(function (meiAbbViePortal) {
    var travel;
    (function (travel) {
        var invoice;
        (function (invoice) {
            var form = /** @class */ (function () {
                function form() {
                }
                ;
                form.prototype.init = function () {
                    var banner = new meiAbbViePortal.ui.statusBanner;
                    window.selectAmountValue = function (index) {
                        var elem = document.getElementsByClassName("ej-numeric")[index];
                        elem.select();
                    };
                    var keys = new meiAbbViePortal.common.keyboard;
                    document.addEventListener("keydown", keys.keyDownTextField, false);
                    banner.init();
                };
                return form;
            }());
            invoice.form = form;
        })(invoice = travel.invoice || (travel.invoice = {}));
    })(travel = meiAbbViePortal.travel || (meiAbbViePortal.travel = {}));
})(meiAbbViePortal || (meiAbbViePortal = {}));
