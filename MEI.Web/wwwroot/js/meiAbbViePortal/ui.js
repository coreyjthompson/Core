var meiAbbViePortal;
(function (meiAbbViePortal) {
    var ui;
    (function (ui) {
        var statusBanner = /** @class */ (function () {
            function statusBanner() {
            }
            ;
            statusBanner.prototype.init = function () {
                // initiate the toggle button of the invoice activity drawer
                //TODO: Move this toggle into the UI code and then fire the UI code from 
                var toggle = document.getElementsByClassName("activity-drawer-toggle")[0];
                if (toggle != null) {
                    var banner = new meiAbbViePortal.ui.statusBanner;
                    toggle.addEventListener("click", banner.toggleStatusBannerActivityDrawer);
                }
            };
            // function: opens and closes the history drawer on the status banner
            statusBanner.prototype.toggleStatusBannerActivityDrawer = function (elem) {
                var drawer = document.getElementsByClassName("activity-drawer")[0];
                if (drawer.classList.contains('is-open')) {
                    // close the drawer by removing is-open class and replace it with is-closed
                    drawer.classList.remove("is-open");
                    drawer.classList.add("is-closed");
                    // change the text
                    elem.innerHTML = "View History";
                }
                else {
                    // open the drawer by removing is-closed class and replace it with is-open
                    drawer.classList.remove("is-closed");
                    drawer.classList.add("is-open");
                    // change the text
                    elem.innerHTML = "Hide";
                }
            };
            return statusBanner;
        }());
        ui.statusBanner = statusBanner;
    })(ui = meiAbbViePortal.ui || (meiAbbViePortal.ui = {}));
})(meiAbbViePortal || (meiAbbViePortal = {}));
