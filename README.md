CR_FluentPoco
=============

CodeRush expansion plugin to turn a list of private fields into fluent methods.

I created this to help me build helper classes for an automated testing framework I was making with Selenium and it's WebDriver.

####Beginning Class####

```
public class Person
{
    private string lastName;
    private string firstName;
}
```

####After applying####

Place the caret on either of the fields and hit your CodeRush activator shortcut (mine is set to `Ctrl - ~` <-- that's a tilde). You can also assign a shortcut to this. Select `Create Fluent Poco Stuff`

```
public class Person
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
   
   public Person WithLastName(string lastName)
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
   
   public Person WithFirstName(string firstName)
   {
       this.firstName = firstName;
       return this;
   }
}

public class PersonHelper
{
    public static Person Create()
    {
        return new Person();
    }
}
```

####How we use the fluent methods####

`PersonHelper.Create().WithFirstName("John").WithLastName("Doe").Continue();`

Note: The `Continue()` method is where we actually setup all of our Selenium magic and use the properties that were created.

####TODO and Construction Notes####

* fields that are of List<T> should actually do the following:

If the field looks like:
`private List<T> things;`, I would like the field to actually change to `private List<T> things = new List<T>();`

The fluent-with method should look like:

```
public ThingHelper FillThings(Thing thing){    things.Add(thing);    return this;}
```
As for the getter...I am not sure what I want that to look like yet.
