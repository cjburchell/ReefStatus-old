
UpdateLockEnabled = function (result) {
    access.LockEnabled = result;
}

UpdateLock = function (result) {
    access.serverIslocked = !result;
    if (access.serverIslocked && access.LockEnabled) {
        if (localStorage.Password != undefined && localStorage.Password != '') {
            access.Unlock(localStorage.Password);
        }
    }
}

UnlockCallback = function (result) {
    access.serverIslocked = !result;
    if (!result) {
        localStorage.Password = undefined;
    }
}

function Access() {

    this.Update = function () {
        Request("command/unlock", "", "UpdateLock);
    }


    this.Lock = function () {
        TextCommand("command/lock);
        localStorage.Password = "";
    }

    this.Unlock = function (password) {
        localStorage.Password = password;
        Request("command/unlock", "password=" + password, "UnlockCallback);
    }

    this.IsUnlocked = function() {
        if (this.LockEnabled) {
            return !this.serverIslocked;
        }

        return true;
    }

    Request("command/lockenabled", "", "UpdateLockEnabled);
    this.Update();

    this.LockEnabled = true;
    this.serverIslocked = true;
}

var access = new Access();