
async function ConnectToNotificationsHub(userName) {
    const reconnectWaitTime = 5 * 1000;
    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationshub?userName=" + userName)
        .build();
    connection.on("NewMessage", writtenFrom => {
        const ele = document.getElementById('pendingMsgs');
        const new_num = parseInt(ele.innerHTML.replace(' unread messages', '')) + 1;
        ele.innerHTML = new_num + ' unread messages';
    });
    
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