var isCtrl = false;
document.onkeyup = function(e) {
	if (e.keyCode == 17) isCtrl = false;
};

document.onkeydown = function(e) {
	if (e.keyCode == 17) isCtrl = true;
	if (e.keyCode == 83 && isCtrl == true) {
		//run code for CTRL+S -- ie, save!
		console.log(shortcuts.Ctrl['S']);
		if (shortcuts.Ctrl['S']) {
			shortcuts.Ctrl['S'].action();
		}
		return false;
	}
};

var shortcuts = {
	Ctrl: [],
	Shift: [],
	CtrlShift: []
};

function RegisterShortcut(keys, action) {
	shortcuts[keys.funxion][keys.key] = { key: keys.key, action : action };
}