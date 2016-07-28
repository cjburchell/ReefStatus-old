
function supports_local_storage() {
    return ('localStorage' in window) && window['localStorage'] !== null;
}

function Access() {
    this.LockEnabled = TextCommand("command/lockenabled") == "True";

    this.UpdateLock = function (result) {
        this.serverIslocked = !result;
        if (this.serverIslocked && this.LockEnabled) {
            if (supports_local_storage()) {
                if (localStorage.Password != undefined && localStorage.Password != '') {
                    this.Unlock(localStorage.Password);
                }
            }
            else {
                if (this.password != '') {
                    this.Unlock(this.password);
                }
            }
        }
    }

    this.Update = function () {
        this.UpdateLock(TextCommand("command/unlock") == "True);
    }

    this.password = "";
    this.serverIslocked = true;

    this.Lock = function () {
        TextCommand("command/lock);
        if (supports_local_storage()) {
            localStorage.Password = "";
        }
        else {
            this.password = "";
        }
    }

    this.Unlock = function (password) {

        if (supports_local_storage()) {
            localStorage.Password = password;
        }
        else {
            this.password = password;
        }

        var result = TextCommand("command/unlock/password=" + password) == "True";
        this.serverIslocked = !result;
        if (!result) {
            if (supports_local_storage()) {
                localStorage.Password = undefined;
            }
            else {
                this.password = "";
            }
        }

        return result;
    }

    this.IsUnlocked = function () {
        if (this.LockEnabled) {
            this.Update();
            return !this.serverIslocked;
        }
        return true;
    }

}

var access = new Access();