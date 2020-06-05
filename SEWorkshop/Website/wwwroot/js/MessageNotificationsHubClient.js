
async function ConnectToNotificationsHub(userName, hubPath, eventNameToFunctionLst) {
    const reconnectWaitTime = 5 * 1000;
    let connection = new signalR.HubConnectionBuilder()
        .withUrl(hubPath + "?userName=" + userName)
        .build();
    for (let i = 0; i < eventNameToFunctionLst.length; i++) {
        const eventName = eventNameToFunctionLst[i][0];
        const func = eventNameToFunctionLst[i][1];
        connection.on(eventName, func);
    }
    
    connection.onclose(async () => {
        console.warn("WS connection closed, try reconnecting with loop interval ${reconnectWaitTime}");
        tryReconnect(connection);
    });
    await tryReconnect(connection);

    async function tryReconnect(connection) {
        try {
            let started = await connection.start();
            console.log('WS client connected!');
            return started;
        } catch (e) {
            await new Promise(resolve => setTimeout(resolve, reconnectWaitTime));
            return await tryReconnect(connection);
        }
    }
}

async function connectToAllHubs(userName) {
    const handlersForMsgHub = [
        ["NewMessage", writtenFrom => {
            const ele = document.getElementById('pendingMsgs');
            const new_num = parseInt(ele.innerHTML.replace(' unread messages', '')) + 1;
            ele.innerHTML = new_num + ' unread messages';
        }]
    ];
    ConnectToNotificationsHub(userName, "/notificationshub", handlersForMsgHub);

    const handlersForPurchasHub = [
        ["NewPurchase", storeName => {
            alert("Someone purchased from your store " + storeName);
        }]
    ];
    ConnectToNotificationsHub(userName, "/purchasenotificationshub", handlersForPurchasHub);

    const handlerForOwnershupHub = [
        ["NewOwnershipRequest", showMsg => {
            alert(showMsg);
        }]
    ];
    ConnectToNotificationsHub(userName, "/ownershiprequesthub", handlerForOwnershupHub);
}