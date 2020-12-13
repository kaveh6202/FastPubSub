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
to subscribe messages to Channel
```c#
var publisher = factory.GetPublisher();
publisher.Publish("Hello World!");
```

## Get IPublisher 
to publish messages to Channel
```c#
var subscriptionHandler = factory.GetSubscriptionHandler();
subscriptionHandler.Subscribe(item => { //do something with recieved message });
```
***

# IOC Friendly
You can simply register IPublisher and ISubscriptionHandler in your DI-Container and then Inject them in your classes
```c#
services.AddTransient<IPublisher>(p => new factory.GetPublisher()));
services.AddTransient<ISubscriptionHandler>(p => new factory.GetSubscriptionHandler()));
```
and use it like this
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
