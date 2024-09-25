module MockBrokers.DevicesBrokerMock

open Motsoft.Util

open Model
open DI.Interfaces

type DevicesBrokerMock (throwError: bool) as this =

    // -----------------------------------------------------------------------------------------------------------------
    let self = this :> IDevicesBroker

    new () = DevicesBrokerMock (throwError = false)
    // -----------------------------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------------------------
    interface IDevicesBroker with

        // -------------------------------------------------------------------------------------------------------------
        member _.getDeviceInfoOrEx () =

            throwError |> failWithIfTrue $"{self.getDeviceInfoOrEx}: Mock Exception"

            """{
              "blockdevices": [
                {
                  "name": "sda",
                  "kname": "sda",
                  "ro": false,
                  "type": "disk",
                  "mountpoint": "/",
                  "label": "root",
                  "path": "/dev/sda",
                  "fstype": "ext4",
                  "parttypename": "Linux",
                  "size": "50G",
                  "children": [
                    {
                      "name": "sda1",
                      "kname": "sda1",
                      "ro": false,
                      "type": "part",
                      "mountpoint": "/boot",
                      "label": "boot",
                      "path": "/dev/sda1",
                      "fstype": "ext4",
                      "parttypename": "Linux filesystem",
                      "size": "500M"
                    },
                    {
                      "name": "sda2",
                      "kname": "sda2",
                      "ro": false,
                      "type": "part",
                      "mountpoint": "/",
                      "label": "root",
                      "path": "/dev/sda2",
                      "fstype": "ext4",
                      "parttypename": "Linux filesystem",
                      "size": "45G"
                    }
                  ]
                },
                {
                  "name": "sdb",
                  "kname": "sdb",
                  "ro": false,
                  "type": "disk",
                  "mountpoint": "",
                  "label": "",
                  "path": "/dev/sdb",
                  "fstype": "",
                  "parttypename": "",
                  "size": "10G",
                  "children": []
                }
              ]
            }"""
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.mountDeviceOrEx (_: SnapshotDevice) =

            throwError |> failWithIfTrue $"{self.mountDeviceOrEx}: Mock Exception"

            Directory.create "/run/homeshift/_dummy_pid"
        // -------------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------------
        member _.unmountCurrentOrEx () =

            throwError |> failWithIfTrue $"{self.unmountCurrentOrEx}: Mock Exception"

            ()
        // -------------------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------------------
