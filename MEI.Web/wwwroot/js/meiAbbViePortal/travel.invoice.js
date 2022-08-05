var meiAbbViePortal;
(function (meiAbbViePortal) {
    var travel;
    (function (travel) {
        var invoice = /** @class */ (function () {
            function invoice() {
            }
            ;
            invoice.prototype.init = function () {
                // initiate the toggle button of the history drawer
                var toggle = document.getElementsByClassName("history-drawer-toggle")[0];
                if (toggle != null) {
                    var banner = new meiAbbViePortal.ui.statusBanner;
                    toggle.addEventListener("click", banner.toggleHistoryDrawer);
                }
            };
            return invoice;
        }());
        travel.invoice = invoice;
    })(travel = meiAbbViePortal.travel || (meiAbbViePortal.travel = {}));
})(meiAbbViePortal || (meiAbbViePortal = {}));
(function (meiAbbViePortal) {
    var travel;
    (function (travel) {
        var invoice;
        (function (invoice_1) {
            var form = /** @class */ (function () {
                function form() {
                }
                ;
                form.prototype.init = function () {
                    var invoice = new meiAbbViePortal.travel.invoice;
                    window.selectAmountValue = function (index) {
                        var elem = document.getElementsByClassName("ej-numeric")[index];
                        elem.select();
                    };
                    var keys = new meiAbbViePortal.common.keyboard;
                    document.addEventListener("keydown", keys.keyDownTextField, false);
                    invoice.init();
                };
                return form;
            }());
            invoice_1.form = form;
        })(invoice = travel.invoice || (travel.invoice = {}));
    })(travel = meiAbbViePortal.travel || (meiAbbViePortal.travel = {}));
})(meiAbbViePortal || (meiAbbViePortal = {}));
//# sourceMappingURL=travel.invoice.js.map