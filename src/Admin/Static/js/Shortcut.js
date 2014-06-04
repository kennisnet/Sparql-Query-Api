var isCtrl = false;
document.onkeyup = function(e) {
	if (e.keyCode == 17) {
		isCtrl = false;
	}
};

document.onkeydown = function(e) {
	if (e.keyCode == 17) {
		isCtrl = true;
	}
	var key = String.fromCharCode(e.keyCode);
	if (isCtrl === true) {
		if (shortcuts.Ctrl[key]) {
			console.log('Ctrl+' + key, shortcuts.Ctrl[key]);
			shortcuts.Ctrl[key].action();
		}
		return false;
	}
	return true;
};

var shortcuts = {
	Ctrl: [],
	Shift: [],
	CtrlShift: []
};

function RegisterShortcut(keys, action) {
	shortcuts[keys.funxion][keys.key] = { key: keys.key, action : action };
}