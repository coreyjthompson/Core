interface Window {
  selectAmountValue: any;
}

namespace meiAbbViePortal.travel {
        export class invoice {
			constructor(){};

    }
}

namespace meiAbbViePortal.travel.invoice {
        export class form {
			constructor(){};

			public init() { 
				var banner = new meiAbbViePortal.ui.statusBanner; 
				window.selectAmountValue = (index) => {
					var elem = document.getElementsByClassName("ej-numeric")[index] as HTMLInputElement;
					elem.select();
				};

				var keys = new meiAbbViePortal.common.keyboard;
				document.addEventListener("keydown", keys.keyDownTextField, false);
				banner.init();
			}
    }
} 