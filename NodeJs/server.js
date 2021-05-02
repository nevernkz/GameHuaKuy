const websocket = require('ws');
const wss = new websocket.Server({port:5500}, ()=>{
    console.log("Server is running.");
});
const sqlite3 = require('sqlite3').verbose();
let db = new sqlite3.Database('./database/gameDB.db',sqlite3.OPEN_CREATE|sqlite3.OPEN_READWRITE,(err)=>{
    if(err) throw err;

    console.log('Connected to database.');   
        
});

var indexRoom = 0;
var wsList = [];
var roomList= [];
var roomMap = new Map();

wss.on("connection",(ws)=>{
    console.log("Client connected.");
    wsList.push(ws);

    ws.on("message",(data)=>{

        var toJsonObj = JSON.parse(data);
        console.log("Message from Client : " + data);

        if(toJsonObj.eventName == "Login"){
            LoginData(ws, toJsonObj.userName, toJsonObj.password);
        }
        else if(toJsonObj.eventName == "Regis"){
            RegisData(toJsonObj.userName, toJsonObj.name, toJsonObj.password);
        }
        else if(toJsonObj.eventName == "CreateRoom"){
            CreateRoom(ws,toJsonObj.roomOption);
        }    
        else if(toJsonObj.eventName == "JoinRoom"){
            JoinRoom(ws,toJsonObj.roomOption);
        }
        else if(toJsonObj.eventName == "LeaveRoom"){
            LeaveRoom(ws,toJsonObj.roomOption);
        }
    
    });

    ws.on("close", ()=>{
        LeaveRoom(ws, (status, roomKey)=>{
            if(status === true){
                if(roomMap.get(roomKey).wsList.size <= 0){
                    roomMap.delete(roomKey);
                }
            }
        });
    });
});

function LoginData(ws, userName, password){
    db.all("SELECT * FROM UserData WHERE PlayerID ='"+userName+"' AND Password = '"+password+"'",(err)=>{
        if(err) throw err;
        
        else{
            let callbackMsg = {
                eventName:"Login",
                status: true
            }

            ws.send(JSON.stringify(callbackMsg));
        }
    });
}

function RegisData(userName, password, name){

    db.all("INSERT INTO UserData (PlayerID, Password, Name, Score, Money) VALUES ('"+userName+"','"+name+"','"+password+"','0','0')", (err)=>{
        if(err) throw err;
    });
}

function CreateRoom(ws,roomOption){
    db.all("INSERT INTO RoomOption (RoomName, RoomPassword) VALUES ('"+roomOption.roomName+"','"+roomOption.roomPassword+"')", (err)=>{
        if(err) {            
            var callbackMsg = {
                eventName: "CreateRoom",
                status:false
            }
    
            ws.send(JSON.stringify(callbackMsg));

            throw err;
        }

        else{
            let roomName = roomOption.roomName;
        roomMap.set(roomName,{
            wsList: new Map()
        });

        roomMap.get(roomName).wsList.set(ws, {});

        var callbackMsg = {
            eventName: "CreateRoom",
            status:true,
            data: JSON.stringify(roomOption)
        }

        ws.send(JSON.stringify(callbackMsg));
        }
    });
    /*var isFoundRoom = roomMap.has(roomOption.roomName);

    if(isFoundRoom === true){
        var callbackMsg = {
            eventName: "CreateRoom",
            status:false
        }

        ws.send(JSON.stringify(callbackMsg));
    }

    else{
        let roomName = roomOption.roomName;
        roomMap.set(roomName,{
            wsList: new Map()
        });

        roomMap.get(roomName).wsList.set(ws, {});

        var callbackMsg = {
            eventName: "CreateRoom",
            status:true,
            data: JSON.stringify(roomOption)
        }

        ws.send(JSON.stringify(callbackMsg));
    }*/
}

function JoinRoom(ws, roomOption){

    db.all("SELECT * FROM RoomOption WHERE RoomName = '"+roomOption.roomName+"' AND RoomPassword = '"+roomOption.roomPassword+"'", (err)=>{
        if(err){
            let callbackMsg = {
                eventName: "JoinRoom",
                status:false
            }

            ws.send(JSON.stringify(callbackMsg));

            throw err;
        }
        else{
            let isFoundClientInRoom = roomMap.get(roomOption.roomName).wsList.has(ws);
            var callbackMsg ={
                eventName: "JoinRoom",
                status: false,
                roomName: undefined
            }
            if(isFoundClientInRoom === true){
                callbackMsg.status = true;
            }
            else{
                roomMap.get(roomName).wsList.set(ws,{});
                callbackMsg.status = true;
    
                callbackMsg.roomName = JSON.stringify(roomList[indexRoom].roomOption);                
            }

            ws.send(JSON.stringify(callbackMsg));
        }
    });
    /*let roomName = roomOption.roomName;
    let isFoundRoom = roomMap.has(roomName);

    var callbackMsg = {
        eventName: "JoinRoom",
        status:false
    }

    if(isFoundRoom === false){

        callbackMsg.status = false;
    }
    else{
        
        let isFoundClientInRoom = roomMap.get(roomName).wsList.has(ws);

        if(isFoundClientInRoom === true){
            callbackMsg.status = false;
        }
        else{
            roomMap.get(roomName).wsList.set(ws,{});
            callbackMsg.status = true;

            callbackMsg.roomName = JSON.stringify(roomList[indexRoom].roomOption);
        }                
    }

    ws.send(JSON.stringify(callbackMsg));*/
}

function LeaveRoom(ws, roomOption){
   
    for(let roomKey of roomMap.keys()){
        if(roomMap.get(roomKey).wsList.has(ws)){
           roomMap.get(roomKey).wsList.delete(ws), roomKey;       
                  
        }
        
        if(roomMap.get(roomKey).wsList.size <= 0){
            roomMap.delete(roomKey);
            var callbackMsg = {
                eventName: "LeaveRoom",
                status: false
            }
            ws.send(JSON.stringify(callbackMsg));
        }
    }

    

    if(roomOption.playerNum <= 0)
            db.all("DELETE FROM RoomOption WHERE RoomName = '"+roomOption.roomName+"'",(err)=>{
                if(err) throw err;

                else{
                    var callbackMsg = {
                        eventName: "LeaveRoom",
                        status: true
                    }
                    ws.send(JSON.stringify(callbackMsg));
                }
            });            
  
}

/*function Broadcast(data){
    for(var i = 0; i < wsList.length; i++){
        wsList[i].send(data);
    }
}*/