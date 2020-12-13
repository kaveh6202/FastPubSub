# FastPubSub
Simple and Fast PubSub Utility 

## The solution contains 3 projects 
* **PubSub** is the core of the utility
* **PubSub.Tests** contains unit tests for some of the functionalities
* **SnakeSampleProject** is a very simple demonstration built around the PubSub Util

## Simple factory
the following exmaple shows how to create a simple factory
```c#
var factory = new PubSubFactory();
```

## Get ISubscriptionHandler 
to subscribe for messages
```c#
var subscriptionHandler = factory.GetSubscriptionHandler();
subscriptionHandler.Subscribe(item => { /*do something with recieved message */ });
```

### Subscription Filters
filter the messages based on type
```c#
 subscriptionHandler.Subscribe<SomeType>(this, item => {/*do something with recieved message */});
```
***note :***
it's good practice to send **subscriber object reference** to Subscribe method , it let the util to keep track of the subscribers aliveness


using filter function for more specific filtering
```c#
subscriptionHandler.Subscribe<SomeType>(this,callback: item => {/*do something with recieved message */},filter: item=>item.Value == 2);
```


## Get IPublisher 
to publish messages to Specific Channel
```c#
var publisher = factory.GetPublisher();
publisher.Publish("Hello World!");
```
***

## Channels 
Utility has a built in default Channel with type ***DefaultChannel*** which is inherited from abstract class ***BaseChannel***

you are free to register as many channels as you want with the following syntax
```c#
class MyChannel : BaseChannel {}
```

using factory to init this channel
```c#
var myChannelFactory = new PubSubFactory<MyChannel>();
subscriptionHandler = myChannelFactory.GetSubscriptionHandler();
var publisher = myChannelFactory.GetPublisher();
```


***

# IOC Friendly
You can simply register IPublisher and ISubscriptionHandler in your DI-Container and then Inject them in your classes

if you did not defined additional channels you can simply register IPubsliher and ISubscriptionHandler with default factory
```c#
services.AddTransient<IPublisher>(p => new factory().GetPublisher()));
services.AddTransient<ISubscriptionHandler>(p => new factory().GetSubscriptionHandler()));
```
```c#
class PublisherClass
{
   private readonly IPublisher _publisher;
   PublisherClass(IPublisher publisher)
   {
      _publisher = publisher;
   }
}
```
or if you have multiple channels 
```c#
services.AddTransient<IPubSubFactory<MyChannel>>(p => new factory<MyChannel>()));
services.AddTransient<IPubSubFactory<MyOtherChannel>>(p => new factory<MyOtherChannel>()));
```
```c#
class PublisherClass
{
   private readonly ISubscriptionHandler _subHandler;
   PublisherClass(IPubSubFactory<MyChannel> channelFactory)
   {
      _subHandler = channelFactory.GetSubscriptionHandler();
   }
}
```

### Channel Configuration
Channels behaviour is configurable

it supports these configurations
* **IgnoreCallbackException** : if set ,ignores callback function exceptions - Default behaviour : exceptions are thrown
* **FireAndForgetCallback** : if set , the publisher does not wait for callback functions to finish - Default behaviour : Waits for the callback function to finish
* **InvokeCallbackFunctionsSimultaneously** : if set ,  callback functions invoke simultaneously - Default Behaviour : callback functions invoke one by one

```c#
var factory = new PubSubFactory().IgnoreCallbackException().FireAndForgetCallback();
var channelFactory = new PubSubFactory<MyChannel>().InvokeCallbackFunctionsSimultaneously();
```

***important note*** :
if **InvokeCallbackFunctionsSimultaneously** is set , the exceptions are automatically ignored

