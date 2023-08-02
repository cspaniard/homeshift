namespace DI

module Brokers =

    type IConfigBroker = Brokers.Config.Broker
    type IDevicesBroker = Brokers.Devices.Broker
    type IConsoleBroker = Brokers.Console.Broker
    type IUsersBroker = Brokers.Users.Broker
    type IProcessBroker = Brokers.Process.Broker
