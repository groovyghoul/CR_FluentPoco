CR_FluentPoco
=============

CodeRush expansion plugin to turn a list of private fields into fluent methods.

I created this to help me build helper classes for an automated testing framework I was making with [Selenium](http://docs.seleniumhq.org/) and it's [WebDriver](http://docs.seleniumhq.org/projects/webdriver/).

####Beginning Class####

```
class Person
{
    private string lastName;
    private string firstName;
}
```

####After applying####

Place the caret on either of the fields and hit your CodeRush activator shortcut (mine is set to `Ctrl - ~` <-- that's a tilde). You can also assign a shortcut to this. Select `Create Fluent Poco Stuff`.

```
public class PersonHelper
{
   private string lastName;
   private string firstName;
   
   public string LastName
   {
       get
       {
           return lastName;
       }
   }
   
   public PersonHelper WithLastName(string lastName)
   {
       this.lastName = lastName;
       return this;
   }
   
   public string FirstName
   {
       get
       {
           return firstName;
       }
   }
   
   public PersonHelper WithFirstName(string firstName)
   {
       this.firstName = firstName;
       return this;
   }
}

public class Person
{
    public static PersonHelper Create()
    {
        return new PersonHelper();
    }
}
```

####How we use the fluent methods####

`Person.Create().WithFirstName("John").WithLastName("Doe").Continue();`

Note: The `Continue()` method is where we actually setup all of our Selenium magic and use the properties that were created.

####Requirements####
* DevExpress CodeRush [ [https://www.devexpress.com/Products/CodeRush/](https://www.devexpress.com/Products/CodeRush/ "CodeRush") ]
* following Rory Becker on Twitter [@RoryBecker](http://twitter.com/RoryBecker) :smiley:

####Installation Notes####
1. Download source
2. Open solution in Visual Studio 2013
3. Go to `Properties -> Build -> Output Path` and adjust so that the plugin builds to the `%UserProfile%\Documents\DevExpress\IDE Tools\Community\PlugIns\` folder (Disclaimer: I'm not sure if VS understands special folders such as `%UserProfile%`, so just browse to your `Documents` folder)

####License####
Copyright (c) 2014 Richard O'Neil, contributors.
Released under the MIT license

####TODO and Construction Notes####

* fields that are of List<T> should actually do the following:

If the field looks like:
`private List<T> things;`, I would like the field to actually change to `private List<T> things = new List<T>();`

The fluent-with method should look like:

```
public ThingHelper FillThings(Thing thing)
{
    things.Add(thing);
    return this;
}

```


As for the getter...I am not sure what I want that to look like yet.
