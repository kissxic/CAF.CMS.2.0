﻿using System.Collections.Generic;

namespace CachingFramework.Redis.Contracts.Providers
{
    /// <summary>
    /// A typed geospatial index Provider interface
    /// </summary>
    public interface IGeoProvider
    {
        /// <summary>
        /// Adds the specified geospatial members (latitude, longitude, object) to the specified key.
        /// </summary>
        /// <typeparam name="T">The member type</typeparam>
        /// <param name="key">The redis key.</param>
        /// <param name="members">The members to add.</param>
        /// <returns>The number of elements added to the sorted set, not including elements already existing.</returns>
        int GeoAdd<T>(string key, GeoMember<T>[] members);
        /// <summary>
        /// Adds the specified members to a geospatial index.
        /// </summary>
        /// <typeparam name="T">The member type</typeparam>
        /// <param name="key">The redis key.</param>
        /// <param name="latitude">The member latitude coordinate.</param>
        /// <param name="longitude">The member longitude coordinate.</param>
        /// <param name="member">The member to add.</param>
        /// <returns>The number of elements added to the sorted set, not including elements already existing.</returns>
        int GeoAdd<T>(string key, double latitude, double longitude, T member);
        /// <summary>
        /// Adds the specified members to a geospatial index.
        /// </summary>
        /// <typeparam name="T">The member type</typeparam>
        /// <param name="key">The redis key.</param>
        /// <param name="coordinate">The member coordinates.</param>
        /// <param name="member">The member to add.</param>
        /// <returns>The number of elements added to the sorted set, not including elements already existing.</returns>
        int GeoAdd<T>(string key, GeoCoordinate coordinate, T member);
        /// <summary>
        /// Return Geohash strings representing the position of a member in a geospatial index (where elements were added using GEOADD).
        /// </summary>
        /// <typeparam name="T">The member type</typeparam>
        /// <param name="key">The redis key.</param>
        /// <param name="member">The member.</param>
        string GeoHash<T>(string key, T member);
        /// <summary>
        /// Return the positions (longitude,latitude) of all the specified members of the geospatial index at key.
        /// </summary>
        /// <typeparam name="T">The member type</typeparam>
        /// <param name="key">The redis key.</param>
        /// <param name="members">The members.</param>
        IEnumerable<GeoMember<T>> GeoPosition<T>(string key, T[] members);
        /// <summary>
        /// Return the position (longitude,latitude) of the specified member of the geospatial index at key.
        /// </summary>
        /// <typeparam name="T">The member type</typeparam>
        /// <param name="key">The redis key.</param>
        /// <param name="member">The member.</param>
        /// <returns>NULL if the member does not exists</returns>
        GeoCoordinate GeoPosition<T>(string key, T member);
        /// <summary>
        /// Return the distance between two members in the geospatial index at key.
        /// </summary>
        /// <typeparam name="T">The member type</typeparam>
        /// <param name="key">The redis key.</param>
        /// <param name="member1">The first member.</param>
        /// <param name="member2">The second member.</param>
        /// <param name="unit">The result unit.</param>
        /// <returns>The distance in the given unit or -1 in case of a non-existing member.</returns>
        double GeoDistance<T>(string key, T member1, T member2, Unit unit);
        /// <summary>
        /// Return the members of a geospatial index, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
        /// </summary>
        /// <typeparam name="T">The member type</typeparam>
        /// <param name="key">The redis key.</param>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="unit">The unit.</param>
        /// <param name="count">If greater than 0, limit the results to the first N matching items.</param>
        IEnumerable<GeoMember<T>> GeoRadius<T>(string key, GeoCoordinate center, double radius, Unit unit, int count = 0);
        /// <summary>
        /// Return the members of a geospatial index, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
        /// </summary>
        /// <typeparam name="T">The member type</typeparam>
        /// <param name="key">The redis key.</param>
        /// <param name="latitude">The latitude of the center.</param>
        /// <param name="longitude">The latitude of the center.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="unit">The unit.</param>
        IEnumerable<GeoMember<T>> GeoRadius<T>(string key, double latitude, double longitude, double radius, Unit unit);
        /// <summary>
        /// Return the members of a geospatial index, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
        /// </summary>
        /// <typeparam name="T">The member type</typeparam>
        /// <param name="key">The redis key.</param>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="unit">The unit.</param>
        IEnumerable<GeoMember<T>> GeoRadius<T>(string key, GeoCoordinate center, double radius, Unit unit);
    }
}