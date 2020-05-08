//Init ก่อน
//C:\Users\Armkaiser\Desktop\SpaceShooting_NodeJS>npm init
//C:\Users\Armkaiser\Desktop\SpaceShooting_NodeJS>npm install body-parser
//C:\Users\Armkaiser\Desktop\SpaceShooting_NodeJS>npm install express
//C:\Users\Armkaiser\Desktop\SpaceShooting_NodeJS>npm install mysql
//C:\Users\Armkaiser\Desktop\SpaceShooting_NodeJS>npm install cors (ทำให้ใช้กับ Html ได้)

const express = require("express");
const cors = require("cors");
const app = express();
const server = require("http").Server(app);
const bodyParser = require("body-parser");
const mysql = require("mysql");

const conSql = mysql.createConnection(
    {
        user: process.env.DB_USER,
        password: process.env.DB_PASS,
        database: process.env.DB_NAME,
        socketPath: `/cloudsql/${process.env.CLOUD_SQL_CONNECTION_NAME}`,
    }
);

/*
const conSql = mysql.createConnection({
    host: "localhost",
    user: "root",
    password: "",
    database: "space_shooter" //จะต้องสร้าง Database ใน myphp ก่อน
});*/

var callbackMessage = {
    status: "",
    detail: "", //If success send data
    data: ""
}

ClearCallbackMessage();

conSql.connect((err) =>
{
    if (err) throw err;

    console.log("MySql Connected.");
});

app.use(bodyParser.json());
app.use(cors());

//Post มีการรับข้อมูลจาก App หรือ Unity
app.post("/register", cors(), (req, res) =>
{
    ClearCallbackMessage();

    var username = req.body.username;
    var password = req.body.password;

    var sql = "INSERT INTO playerlogin (Username, Password) VALUES ('" + username + "','" + password + "')";

    conSql.query(sql, (err, result) =>
    {
        if (err)
        {
            callbackMessage.status = "fail";

            if (err.code == 'ER_DUP_ENTRY')
            {
                callbackMessage.detail = "Username already exist.";
                console.log("This is duplicate key");
            }
            else
            {
                callbackMessage.detail = "Some thing wrong.";
                console.log(err);
            }
        }
        else
        {
            var sql = "INSERT INTO playerdata (Username) VALUES ('" + username + "')";

            conSql.query(sql, (err, result) =>
            {
                if (err)
                {
                    callbackMessage.status = "fail";
                    callbackMessage.detail = "Cannot init player data";
                    console.log(err);
                }
                else
                {
                    callbackMessage.status = "success";
                }
            });

            callbackMessage.status = "success";
        }

        res.send(callbackMessage);
    });
});

app.post("/login", cors(), (req, res) =>
{
    ClearCallbackMessage();

    var username = req.body.username;
    var password = req.body.password;

    var selectSql = "SELECT * FROM playerlogin WHERE Username='" + username + "' AND Password='" + password + "'";

    //result ที่ได้ก็จะออกมาเป็น json
    conSql.query(selectSql, (err, result) =>
    {
        if (err)
        {
            callbackMessage.status = "fail";
            callbackMessage.detail = "Cannot connect to database.";
            console.log(err);
        }
        else
        {
            if (result.length > 0)
            {
                callbackMessage.status = "success";
            }
            else
            {
                callbackMessage.status = "fail";
                callbackMessage.detail = "Your username or password is wrong.";
            }
        }

        res.send(callbackMessage);
    });

});

app.post("/PlayerData", cors(), (req, res) =>
{
    ClearCallbackMessage();

    var sqlSelectData = "SELECT * FROM playerdata WHERE Username='" + req.body.username + "'";

    conSql.query(sqlSelectData, (err, result) =>
    {
        if (err)
        {
            callbackMessage.status = "fail";
            callbackMessage.detail = "Cannot get playerdata";
        }
        else
        {
            if(result.length > 0)
            {
                callbackMessage.status = "success";
                callbackMessage.detail = "Get playerdata success";
                callbackMessage.data = result;
            }
        }

        res.send(callbackMessage);
    });
});

app.post("/UpdatePlayer", cors(), (req, res) =>
{
    ClearCallbackMessage();

    var username = req.body.Username;
    var winCount = req.body.WinCount;
    var killCount = req.body.KillCount;
    var deadCount = req.body.DeadCount;
    var playCount = req.body.PlayCount;

    var sqlUpdate = "UPDATE playerdata SET WinCount='"+winCount+"',KillCount='"+killCount+"',DeadCount='"+deadCount+"',PlayCount='"+playCount+"' WHERE Username='"+username+"'";

    conSql.query(sqlUpdate, (err, result)=>
    {
        if(err)
        {
            callbackMessage.status = "fail";
            res.send(err);
        }
        else
        {
            callbackMessage.status = "success";
            callbackMessage.detail = "Update data Successful";
        }

        res.send(callbackMessage);
    })
});

//get แค่เรียกใช้เฉยๆ
/*
app.get("/test", (req, res) =>
{
    console.log(req.body);

    res.send("Hello World ");
});
*/

const PORT = 8080;
server.listen(PORT, () =>
{
    console.log("App listen on port : " + PORT);
});

function ClearCallbackMessage()
{
    callbackMessage.detail = "";
    callbackMessage.status = "";
    callbackMessage.data = null;
}

/*
gcloud sql connect mysql-space-shooter -u root < C:\Users\Armkaiser\Downloads\127_0_0_1.sql

gcloud sql connect mysql-space-shooter -u root

MySQL [(none)]> SHOW DATABASES;

+--------------------+
| Database           |
+--------------------+
| information_schema |
| mygamedb           |
| mysql              |
| performance_schema |
| phpmyadmin         |
| sys                |
| test               |
+--------------------+

MySQL [(none)]> USE mygamedb
Database changed
MySQL [mygamedb]> SHOW TABLES;
+--------------------+
| Tables_in_mygamedb |
+--------------------+
| playerdata         |
+--------------------+
1 row in set (0.077 sec)

MySQL [mygamedb]> SELECT * FROM playerdata;
+------------+------------+-------+
| PlayerID   | PlayerName | Money |
+------------+------------+-------+
| 123456     | test01     |   100 |
| 5555555555 | test1      |   500 |
+------------+------------+-------+
2 rows in set (0.077 sec)

C:\Users\Armkaiser\Desktop\NodeJS_01>gcloud app deploy
C:\Users\Armkaiser\Desktop\NodeJS_01>gcloud app browse
*/