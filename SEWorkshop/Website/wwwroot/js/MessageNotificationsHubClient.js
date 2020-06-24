
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

    const handlersNewReportHub = [
        ["NewUseReport", kind => {
            console.log('got kind of: ' + kind);
            const domTotal = document.getElementById('TotalNum');
            if (domTotal !== null) {
                const currInt = parseInt(domTotal.innerHTML);
                domTotal.innerHTML = currInt + 1;
            }
            const domTotal2 = document.getElementById('SecondTableTotalNum');
            if (domTotal2 !== null) {
                const currInt = parseInt(domTotal2.innerHTML);
                domTotal2.innerHTML = currInt + 1;
            }
            //TODO: SYNC THIS METHOD IF POSSIBLE
            if (kind === 'Guest') {
                const dom = document.getElementById('GuestsNum');
                if (dom !== null) {
                    const currInt = parseInt(dom.innerHTML);
                    dom.innerHTML = currInt + 1;
                }
                const dom2 = document.getElementById('SecondTableGuestsNum');
                if (dom2 !== null) {
                    const currInt = parseInt(dom2.innerHTML);
                    dom2.innerHTML = currInt + 1;
                }
            }
            else if (kind === 'LoggedInNotOwnNotManage') {
                const dom = document.getElementById('LoggedInNotOwnNotManageNum');
                if (dom !== null) {
                    const currInt = parseInt(dom.innerHTML);
                    dom.innerHTML = currInt + 1;
                }
                const dom2 = document.getElementById('SecondTableLoggedInNotOwnNotManageNum');
                if (dom2 !== null) {
                    const currInt = parseInt(dom2.innerHTML);
                    dom2.innerHTML = currInt + 1;
                }
            }
            else if (kind === 'LoggedInNoOwnYesManage') {
                const dom = document.getElementById('LoggedInNoOwnYesManageNum');
                if (dom !== null) {
                    const currInt = parseInt(dom.innerHTML);
                    dom.innerHTML = currInt + 1;
                }
                const dom2 = document.getElementById('SecondTableLoggedInNoOwnYesManageNum');
                if (dom2 !== null) {
                    const currInt = parseInt(dom2.innerHTML);
                    dom2.innerHTML = currInt + 1;
                }
            }
            else if (kind === 'LoggedInYesOwn') {
                const dom = document.getElementById('LoggedInYesOwnNum');
                if (dom !== null) {
                    const currInt = parseInt(dom.innerHTML);
                    dom.innerHTML = currInt + 1;
                }
                const dom2 = document.getElementById('SecondTableLoggedInYesOwnNum');
                if (dom2 !== null) {
                    const currInt = parseInt(dom2.innerHTML);
                    dom2.innerHTML = currInt + 1;
                }
            }
            else if (kind === 'Admin') {
                const dom = document.getElementById('AdminNum');
                if (dom !== null) {
                    const currInt = parseInt(dom.innerHTML);
                    dom.innerHTML = currInt + 1;
                }
                const dom2 = document.getElementById('SecondTableAdminNum');
                if (dom2 !== null) {
                    const currInt = parseInt(dom2.innerHTML);
                    dom2.innerHTML = currInt + 1;
                }
            }
        }]
    ];
    ConnectToNotificationsHub(userName, "/newusereporthub", handlersNewReportHub);
}