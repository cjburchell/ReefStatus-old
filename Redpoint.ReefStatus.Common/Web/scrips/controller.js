function Controller($http, $interval) {

    var controller = this;

    this.getInfo = function() {
        $http.get("controller/info")
            .then(function (response) {
                controller.info = response.data;
                var minutes = 1000 * 60;
                var hours = minutes * 60;
                var days = hours * 24;

                controller.info.Reminders.forEach(function (item) {
                    item.timeLeft = Math.round((new Date(item.Next) - Date.now()) / days);
                });

                controller.info.Reminders.IsOverdue = controller.info.Reminders
                    .some(function (reminder) { return reminder.IsOverdue });

                controller.refreshAssoications();
            });
    }

    this.refreshAssoications = function() {
        var updateAssoications = function(mode) {
            if (mode.IsProbe && controller.probes) {
                mode.Icon = "icons/thermometer.svg";
                mode.Item = controller.probes.find(function(item) { return item.Id === mode.Id;
                });

                if (mode.Item) {
                    mode.ValueString = mode.Item.ConvertedValue.toString() +mode.Item.Units;
                }
                return;
            }

            switch (mode.DeviceMode) {
            case "Lights":
                mode.Icon = "icons/bulb.svg";
                if (controller.lights) {
                    mode.Item = controller.lights.find(function(item) {
                        return item.Id === mode.Id;
                    });

                    if (mode.Item) {
                        mode.ValueString = mode.Item.Value.toString() +mode.Item.Units;
                    }
                }
                return;
            case "Timer":
                 mode.Icon = "icons/timer.svg";
                if (controller.dosingPumps) {
                    mode.Item = controller.dosingPumps.find(function(item) {
                        return item.Id === mode.Id;
                    });

                    if (mode.Item) {
                        mode.Icon = "icons/dropper.svg";
                        mode.ValueString = mode.Item.Value;
                    }
                } 
                return;
            case "Water":
                mode.Icon = "icons/water_level.svg";
                if(controller.levelSensors) {
                    mode.Item = controller.levelSensors.find(function(item) {
                        return item.Id === mode.Id;
                        });

                    if (mode.Item) {
                        mode.ValueString = mode.Item.Value;
                    }
                }
                return;
            case "CurrentPump":
                mode.Icon = "icons/wave.svg";
                if (controller.pumps) {
                    mode.Item = controller.pumps.find(function(item) {
                        return item.Id === mode.Id;
                    });

                    if (mode.Item) {
                        mode.ValueString = mode.Item.Value.toString() +mode.Item.Units;
                    }
                }

                return;
            case "ProgrammableLogic":
                mode.Icon = "icons/puzzle.svg";
                if (controller.programmablelogic) {
                    mode.Item = controller.programmablelogic.find(function(item) {
                        return item.Id === mode.Id;
                    });
                }
                return;
            case "Maintenance":
                mode.Icon = "icons/wrench.svg";
                if(controller.info) {
                    mode.Item = controller.info.Maintenance.find(function(item) {
                        return item.Index === mode.Port;
                    });
                    
                    if (mode.Item) {
                        mode.ValueString = mode.Item.IsActive ? "On": "Off";
                    }
                }
                return;
            case "ThunderStorm":
                mode.Icon = "icons/thunder.svg";
                return;
            case "Thunder":
                mode.Icon = "icons/thunder.svg";
                return;
            case "Alarm":
                mode.Icon = "icons/alarm.svg";
                return;
            case "WaterChange":
                mode.Icon = "icons/water_change.svg";
                return;
            default:
            }
        }

        if (controller.sports) {
            controller.sports.forEach(function(item) {
                updateAssoications(item.Mode);
            });
        }

        if (controller.lports) {
            controller.lports.forEach(function(item) {
                updateAssoications(item.Mode);
            });
        }

        if (controller.programmablelogic) {
            controller.programmablelogic.forEach(function(item) {
                updateAssoications(item.Input1);
                updateAssoications(item.Input2);
            });
        }
    }

    this.getData = function() {
        controller.getInfo();

        $http.get("controller/probe")
            .then(function(response) {
                controller.probes = response.data;
                controller.probes.IsAlarm = controller.probes.some(function (sensor) { return sensor.AlarmState === "On" });
                controller.refreshAssoications();

                controller.probes.forEach(function(item) {
                    item.showGraph = function() {
                        controller.SelectedProbe = item;

                        $http.get("data/log/" + item.Id + "?limit=100")
                            .then(function (response) {
                                controller.SelectedData = [];
                                response.data.forEach(function(dataPoint) {
                                    controller.SelectedData.push({ x: dataPoint.time, y: dataPoint.value });
                                });
                            });
                    }
                });
            });
        $http.get("controller/levelsensor")
            .then(function(response) {
                controller.levelSensors = response.data;
                controller.levelSensors.IsAlarm = controller.levelSensors
                    .some(function(sensor) { return sensor.AlarmState === "On" });

                controller.levelSensors.forEach(function (item) {
                    item.clearAlarm = function() {
                        $http.post("command/clearlevelalarm/" + item.Id)
                            .then(function(response) {
                                if (response.data !== true) {
                                    alert(response.data);
                                }
                            });
                    };

                    item.startWaterChange = function() {
                        $http.post("command/startwaterchange/" + item.Id)
                            .then(function(response) {
                                if (response.data !== true) {
                                    alert(response.data);
                                }
                            });
                    };

                    controller.refreshAssoications();
                });

            });

        $http.get("controller/sport")
            .then(function(response) {
                controller.sports = response.data;
                controller.sports.forEach(function (item) {
                    item.setSocket = function(enable) {
                        $http.post("command/SetSocket/" + item.Id, enable)
                            .then(function(response) {
                                if (response.data !== true) {
                                    alert(response.data);
                                }
                            });

                        item.showLogic = function() {
                            controller.SelectedPort = item;
                        };
                    };
                });
                controller.refreshAssoications();
            });

        $http.get("controller/lport")
            .then(function(response) {
                controller.lports = response.data;
                controller.refreshAssoications();
            });

        $http.get("controller/digitalinput")
            .then(function (response) {
                controller.digitalInput = response.data;
            });

        $http.get("controller/pump")
            .then(function(response) {
                controller.pumps = response.data;
                controller.refreshAssoications();
            });

        $http.get("controller/programmablelogic")
            .then(function (response) {
                controller.programmablelogic = response.data;
                controller.refreshAssoications();
            });

        $http.get("controller/dosingpump")
            .then(function(response) {
                controller.dosingPumps = response.data;

                controller.dosingPumps.forEach(function (item) {
                    item.updateDousingValue = function(perDay, rate) {
                        $http.put("command/updatedousingvalue/" + item.Id, { PerDay: perDay, Rate: rate })
                            .then(function(response) {
                                if (response.data !== true) {
                                    alert(response.data);
                                }
                            });
                    };
                });
                controller.refreshAssoications();
            });

        $http.get("controller/light")
            .then(function(response) {
                controller.lights = response.data;

                controller.lights.forEach(function (item) {
                    item.setLight = function(enable) {
                        $http.post("command/setlight/" + item.Id, enable)
                            .then(function(response) {
                                if (response.data !== true) {
                                    alert(response.data);
                                }
                            });
                    };
                });
                controller.refreshAssoications();

            });
    }

    this.feedPause = function () {
        $http.post("command/feedpasue", true)
            .then(function (response) {
                if (response.data !== true) {
                    alert(response.data);
                }

                controller.getInfo();
            });
    }

    this.manualLights = function () {
        var enable = controller.info.OperationMode !== "ManualIllumination";
        $http.post("command/manuallights", enable)
            .then(function (response) {
                if (response.data !== true) {
                    alert(response.data);
                }

                controller.getInfo();
            });
    }

    this.manualSockets = function () {
        var enable = controller.info.OperationMode !== "ManualSockets";
        $http.post("command/manualSockets", enable)
            .then(function (response) {
                if (response.data !== true) {
                    alert(response.data);
                }

                controller.getInfo();
            });
    }

    this.thunderstorm = function () {
        $http.post("command/thunderstorm", 5)
            .then(function (response) {
                if (response.data !== true) {
                    alert(response.data);
                }

                controller.getInfo();
            });
    }

    this.refresh = function (enable) {
        $http.post("command/refresh", enable)
            .then(function (response) {
                if (response.data !== true) {
                    alert(response.data);
                }

                controller.getData();
            });
    }

    this.getData();
    $interval(this.getData, 10000);
}