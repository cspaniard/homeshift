namespace Brokers.Users

open Model

type private IProcessBroker = DI.Brokers.IProcessBrokerDI

type Broker () =

    // -----------------------------------------------------------------------------------------------------------------
    static member getUserInfoFromPasswordFileOrEx (userName : UserName) =

        IProcessBroker.startProcessAndReadToEndOrEx
            "grep"
            $"^{userName.value}: /etc/passwd"
    // -----------------------------------------------------------------------------------------------------------------
