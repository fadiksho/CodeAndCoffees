using MyBlog.Entity;
using MyBlog.Model;
using System;
using WebPush;

namespace UnitTest.Mapper
{
  public static class ClasseComparer
  {
    public static bool IsBlogEqual(this Blog blog1, Blog blog2)
    {
      if (blog1.Id != blog2.Id)
        throw new Exception("Blog 'Id' are not Equal.");
      if (blog1.Title != blog2.Title)
        throw new Exception("Blog 'Title' are not Equal.");
      if (blog1.Slug != blog2.Slug)
        throw new Exception("Blog 'Slug' are not Equal.");
      if (blog1.IsPublished != blog2.IsPublished)
        throw new Exception("Blog 'IsPublished' are not Equal.");
      if (blog1.PublishedDate != blog2.PublishedDate)
        throw new Exception("Blog 'PublishedDate' are not Equal.");
      if (blog1.Description != blog2.Description)
        throw new Exception("Blog 'Description' are not Equal.");
      if (blog1.Body != blog2.Body)
        throw new Exception("Blog 'Body' are not Equal.");
      if (blog1.Tags.Count != blog2.Tags.Count)
        throw new Exception("Blog 'Tags' are not Equal.");

      return true;
    }

    public static bool IsBlogTableEqualAfterConvertFromBlogForCreatingDto(
      this BlogTable blog1,
      BlogTable blog2)
    {
      if (blog1.Id == blog2.Id)
        throw new Exception("Blog 'Id' should be 0.");
      if (blog1.Title != blog2.Title)
        throw new Exception("Blog 'Title' are not Equal.");
      if (blog1.Slug != blog2.Slug)
        throw new Exception("Blog 'Slug' are not Equal.");
      if (blog1.IsPublished != blog2.IsPublished)
        throw new Exception("Blog 'IsPublished' are not Equal.");
      if (blog1.PublishedDate != blog2.PublishedDate)
        throw new Exception("Blog 'PublishedDate' are not Equal.");
      if (blog1.Description != blog2.Description)
        throw new Exception("Blog 'Description' are not Equal.");
      if (blog1.Body != blog2.Body)
        throw new Exception("Blog 'Body' are not Equal.");
      if (blog1.Tags != blog2.Tags)
        throw new Exception("Blog 'Tags' are not Equal.");
      if (blog1.KeyWords != blog2.KeyWords)
        throw new Exception("Blog 'Key Words' are not Equal.");

      return true;
    }

    public static bool IsBlogTableEqualAfterConvertFromBlogForUpdatingDto(this BlogTable blog1, BlogTable blog2)
    {
      if (blog1.Id == blog2.Id)
        throw new Exception("Blog 'Id' should be 0.");
      if (blog1.Title != blog2.Title)
        throw new Exception("Blog 'Title' are not Equal.");
      if (blog1.Slug != blog2.Slug)
        throw new Exception("Blog 'Slug' are not Equal.");
      if (blog1.IsPublished != blog2.IsPublished)
        throw new Exception("Blog 'IsPublished' are not Equal.");
      if (blog1.PublishedDate != blog2.PublishedDate)
        throw new Exception("Blog 'PublishedDate' are not Equal.");
      if (blog1.Description != blog2.Description)
        throw new Exception("Blog 'Description' are not Equal.");
      if (blog1.Body != blog2.Body)
        throw new Exception("Blog 'Body' are not Equal.");
      if (blog1.Tags != blog2.Tags)
        throw new Exception("Blog 'Tags' are not Equal.");
      if (blog1.KeyWords != blog2.KeyWords)
        throw new Exception("Blog 'Key Words' are not Equal.");

      return true;
    }


    public static bool IsPushNontificationTableEqual(
      this PushNotificationSubscriptionTable pushNotificationTable1,
      PushNotificationSubscriptionTable pushNotificationTable2)
    {
      if (pushNotificationTable1.Id == pushNotificationTable2.Id)
        throw new Exception("PushNotificationSubscriptionTable 'Id' should be 0.");
      if (pushNotificationTable1.AuthSecret != pushNotificationTable2.AuthSecret)
        throw new Exception("PushNotificationSubscriptionTable 'AuthSecret' are not Equal");
      if (pushNotificationTable1.EndPoint != pushNotificationTable2.EndPoint)
        throw new Exception("PushNotificationSubscriptionTable 'EndPoint' are not Equal");
      if (pushNotificationTable1.Key != pushNotificationTable2.Key)
        throw new Exception("PushNotificationSubscriptionTable 'Key' are not Equal");

      return true;
    }

    public static bool IsPushSubscriptionEqual(
      this PushSubscription pushSubscription1,
      PushSubscription pushSubscription2)
    {
      if (pushSubscription1.Endpoint != pushSubscription2.Endpoint)
        throw new Exception("PushSubscription 'Endpoint' are not Equal");
      if (pushSubscription1.Auth != pushSubscription2.Auth)
        throw new Exception("PushSubscription 'Auth' are not Equal");
      if (pushSubscription1.P256DH != pushSubscription2.P256DH)
        throw new Exception("PushSubscription 'P256DH' are not Equal");

      return true;
    }
  }
}
