# FastPubSub
Simple and Fast PubSub Utility 

## The solution contains 3 projects 
* **PubSub** is the core of the utility
* **PubSub.Tests** contains unit tests for some of the functionalities
* **SnakeSampleProject** is a very simple demonstration of what the util can do

## Simple factory
the following exmaple shows how to create a simple factory
```c#
var factory = new PubSubFactory().FireAndForgetCallback().IgnoreCallbackException();
```

## Get ISubscriptionHandler 
to subscribe for channel messages
```c#
var subscriptionHandler = factory.GetSubscriptionHandler();
subscriptionHandler.Subscribe(item => { //do something with recieved message });
```

### Subscription Filters
filter the messages based on their types
```c#
 subscriptionHandler.Subscribe<SomeType>(this, item => {//do some thing with the message});
```
using filter function for more specific filtering
```c#
subscriptionHandler.Subscribe<SomeType>(this, item => {//do some thing with the message},item=>item.Value == 2);
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
var myChannelFactory = new PubSubFactory<MyChannel>().IgnoreCallbackException();
subscriptionHandler = myChannelFactory.GetSubscriptionHandler();
var publisher = myChannelFactory.GetPublisher();
```


***

# IOC Friendly
You can simply register IPublisher and ISubscriptionHandler in your DI-Container and then Inject them in your classes

if you did not defined additional channels you can simply register IPubsliher and ISubscriptionHandler with default factory
```c#
services.AddTransient<IPublisher>(p => new factory().IgnoreCallbackException().GetPublisher()));
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
