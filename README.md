# api32-net-client
.NET client for [`Api32`](https://github.com/abobija/api32)

![](doc/img/napi32-ecosystem.png)

## Demo

## Demo
[![C# controls ESP32 over wifi (with NApi32 REST client)](https://img.youtube.com/vi/1eqhiDS0irI/mqdefault.jpg)](https://www.youtube.com/watch?v=1eqhiDS0irI)


## Usage

Let's suppose that you have published REST API with one **POST** `/led` endpoint which resposibility is to control the state of LED.

*`init.lua`*

```
api = require('api32')
    .create({
        auth = {
            user = 'master',
            pwd  = 'api32secret'
        }
    })
    .on_post('/led', function(jreq)
        if jreq ~= nil and jreq.State ~= nil then
            gpio.write(BLUE_LED, jreq.State)
        end

        return {
            Device = 'Blue LED',
            State  = gpio.read(BLUE_LED)
        }
    end)
```

Then your C# application for turning the LED ON should looks something like this:

*`Program.cs`*

```
class Program
{
    static void Main(string[] args)
    {
        var client = new Api32Client("http://192.168.0.104")
            .Authorize("master", "api32secret");
        
        // Making the request for the turning LED ON
        var ledCtrlReq = new LedRequest { State = 1 };
        
        // Send request and get response object from Api32
        var ledCtrlRes = client.DoPost<LedResponse>("/led", ledCtrlReq);
    }
}

public class LedRequest
{
    public int State { get; set; }
}

public class LedResponse
{
    public string Device { get; set; }
    public int State { get; set; }
}
```

Simple, clean and pretty cool, right? :v:
