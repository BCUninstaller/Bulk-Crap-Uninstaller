/*
 * DateTimeClusteringStrategy - A strategy to cluster objects by a date time
 *
 * Author: Phillip Piper
 * Date: 30-March-2011 9:40am
 *
 * Change log:
 * 2011-03-30  JPP  - First version
 * 
 * Copyright (C) 2011-2014 Phillip Piper
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact phillip.piper@gmail.com.
 */

using System;
using System.Globalization;

namespace BrightIdeasSoftware {

    /// <summary>
    /// This enum is used to indicate various portions of a datetime
    /// </summary>
    [Flags]
    public enum DateTimePortion {
        /// <summary>
        /// Year
        /// </summary>
        Year = 0x01,
        
        /// <summary>
        /// Month
        /// </summary>
        Month = 0x02,
        
        /// <summary>
        /// Day of the month
        /// </summary>
        Day = 0x04,
        
        /// <summary>
        /// Hour
        /// </summary>
        Hour = 0x08,
        
        /// <summary>
        /// Minute
        /// </summary>
        Minute = 0x10,

        /// <summary>
        /// Second
        /// </summary>
        Second = 0x20
    }

    /// <summary>
    /// This class implements a strategy where the model objects are clustered
    /// according to some portion of the datetime value in the configured column.
    /// </summary>
    /// <remarks>To create a strategy that grouped people who were born in
    /// the same month, you would create a strategy that extracted just
    /// the month, and formatted it to show just the month's name. Like this:
    /// </remarks>
    /// <example>
    /// someColumn.ClusteringStrategy = new DateTimeClusteringStrategy(DateTimePortion.Month, "MMMM");
    /// </example>
    public class DateTimeClusteringStrategy : ClusteringStrategy {
        #region Life and death

        /// <summary>
        /// Create a strategy that clusters by month/year
        /// </summary>
        public DateTimeClusteringStrategy()
            : this(DateTimePortion.Year | DateTimePortion.Month, "MMMM yyyy") {
        }

        /// <summary>
        /// Create a strategy that clusters around the given parts
        /// </summary>
        /// <param name="portions"></param>
        /// <param name="format"></param>
        public DateTimeClusteringStrategy(DateTimePortion portions, string format) {
            Portions = portions;
            Format = format;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the format string will will be used to create a user-presentable
        /// version of the cluster key.
        /// </summary>
        /// <remarks>The format should use the date/time format strings, as documented
        /// in the Windows SDK. Both standard formats and custom format will work.</remarks>
        /// <example>"D" - long date pattern</example>
        /// <example>"MMMM, yyyy" - "January, 1999"</example>
        public string Format {
            get { return format;  }
            set { format = value;  }
        }
        private string format;

        /// <summary>
        /// Gets or sets the parts of the DateTime that will be extracted when
        /// determining the clustering key for an object.
        /// </summary>
        public DateTimePortion Portions {
            get { return portions;  }
            set { portions = value;  }
        }
        private DateTimePortion portions = DateTimePortion.Year | DateTimePortion.Month;

        #endregion

        #region IClusterStrategy implementation

        /// <summary>
        /// Get the cluster key by which the given model will be partitioned by this strategy
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override object GetClusterKey(object model) {
            // Get the data attribute we want from the given model
            // Make sure the returned value is a DateTime
            if (!(Column.GetValue(model) is DateTime dateTime))
                return null;

            // Extract the parts of the datetime that we are intereted in.
            // Even if we aren't interested in a particular portion, we still have to give it a reasonable default
            // otherwise we won't be able to build a DateTime object for it
            int year = ((Portions & DateTimePortion.Year) == DateTimePortion.Year) ? dateTime.Year : 1;
            int month = ((Portions & DateTimePortion.Month) == DateTimePortion.Month) ? dateTime.Month : 1;
            int day = ((Portions & DateTimePortion.Day) == DateTimePortion.Day) ? dateTime.Day : 1;
            int hour = ((Portions & DateTimePortion.Hour) == DateTimePortion.Hour) ? dateTime.Hour : 0;
            int minute = ((Portions & DateTimePortion.Minute) == DateTimePortion.Minute) ? dateTime.Minute : 0;
            int second = ((Portions & DateTimePortion.Second) == DateTimePortion.Second) ? dateTime.Second : 0;

            return new DateTime(year, month, day, hour, minute, second);
        }

        /// <summary>
        /// Gets the display label that the given cluster should use
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns></returns>
        public override string GetClusterDisplayLabel(ICluster cluster) {
            DateTime? dateTime = cluster.ClusterKey as DateTime?; 

            return ApplyDisplayFormat(cluster, dateTime.HasValue ? DateToString(dateTime.Value) : NULL_LABEL);
        }

        /// <summary>
        /// Convert the given date into a user presentable string
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        protected virtual string DateToString(DateTime dateTime) {
            if (String.IsNullOrEmpty(Format))
                return dateTime.ToString(CultureInfo.CurrentUICulture);

            try {
                return dateTime.ToString(Format);
            }
            catch (FormatException) {
                return String.Format("Bad format string '{0}' for value '{1}'", Format, dateTime);
            }
        }
    
        #endregion
    }
}
