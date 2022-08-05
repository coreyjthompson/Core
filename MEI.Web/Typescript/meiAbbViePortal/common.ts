namespace meiAbbViePortal {
    export namespace common {
        export class keyboard {
			constructor(){};

			// function: prevents the return key from submitting the form
			keyDownTextField(e) {
				var keyCode = e.keyCode;
				if (keyCode==13) {
					event.preventDefault();
				} 
			}
        }
    }
} 