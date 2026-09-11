/*
 * Filters - Filtering on ObjectListViews
 *
 * Author: Phillip Piper
 * Date: 03/03/2010 17:00 
 *
 * Change log:
 * 2011-03-01  JPP  Added CompositeAllFilter, CompositeAnyFilter and OneOfFilter
 * v2.4.1
 * 2010-06-23  JPP  Extended TextMatchFilter to handle regular expressions and string prefix matching.
 * v2.4
 * 2010-03-03  JPP  Initial version
 *
 * TO DO:
 *
 * Copyright (C) 2010-2014 Phillip Piper
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
using System.Collections;
using System.Collections.Generic;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// Interface for model-by-model filtering
    /// </summary>
    public interface IModelFilter
    {
        /// <summary>
        /// Should the given model be included when this filter is installed
        /// </summary>
        /// <param name="modelObject">The model object to consider</param>
        /// <returns>Returns true if the model will be included by the filter</returns>
        bool Filter(object modelObject);
    }

    /// <summary>
    /// Interface for whole list filtering
    /// </summary>
    public interface IListFilter
    {
        /// <summary>
        /// Return a subset of the given list of model objects as the new
        /// contents of the ObjectListView
        /// </summary>
        /// <param name="modelObjects">The collection of model objects that the list will possibly display</param>
        /// <returns>The filtered collection that holds the model objects that will be displayed.</returns>
        IEnumerable Filter(IEnumerable modelObjects);
    }

    /// <summary>
    /// Base class for model-by-model filters
    /// </summary>
    public class AbstractModelFilter : IModelFilter
    {
        /// <summary>
        /// Should the given model be included when this filter is installed
        /// </summary>
        /// <param name="modelObject">The model object to consider</param>
        /// <returns>Returns true if the model will be included by the filter</returns>
        virtual public bool Filter(object modelObject) {
            return true;
        }
    }

    /// <summary>
    /// This filter calls a given Predicate to decide if a model object should be included
    /// </summary>
    public class ModelFilter : IModelFilter
    {
        /// <summary>
        /// Create a filter based on the given predicate
        /// </summary>
        /// <param name="predicate">The function that will filter objects</param>
        public ModelFilter(Predicate<object> predicate) {
            Predicate = predicate;
        }

        /// <summary>
        /// Gets or sets the predicate used to filter model objects
        /// </summary>
        protected Predicate<object> Predicate {
            get { return predicate; }
            set { predicate = value; }
        }
        private Predicate<object> predicate;

        /// <summary>
        /// Should the given model object be included?
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        virtual public bool Filter(object modelObject) {
            return Predicate == null ? true : Predicate(modelObject);
        }
    }

    /// <summary>
    /// A CompositeFilter joins several other filters together.
    /// If there are no filters, all model objects are included
    /// </summary>
    abstract public class CompositeFilter : IModelFilter {

        /// <summary>
        /// Create an empty filter
        /// </summary>
        public CompositeFilter() {
        }

        /// <summary>
        /// Create a composite filter from the given list of filters
        /// </summary>
        /// <param name="filters">A list of filters</param>
        public CompositeFilter(IEnumerable<IModelFilter> filters) {
            foreach (IModelFilter filter in filters) {
                if (filter != null)
                    Filters.Add(filter);
            }
        }

        /// <summary>
        /// Gets or sets the filters used by this composite
        /// </summary>
        public IList<IModelFilter> Filters {
            get { return filters; }
            set { filters = value; }
        }
        private IList<IModelFilter> filters = new List<IModelFilter>();

        /// <summary>
        /// Get the sub filters that are text match filters
        /// </summary>
        public IEnumerable<TextMatchFilter> TextFilters {
            get {
                foreach (IModelFilter filter in Filters) {
                    if (filter is TextMatchFilter textFilter)
                        yield return textFilter;
                }
            }
        }

        /// <summary>
        /// Decide whether or not the given model should be included by the filter
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns>True if the object is included by the filter</returns>
        virtual public bool Filter(object modelObject) {
            if (Filters == null || Filters.Count == 0)
                return true;

            return FilterObject(modelObject);
        }

        /// <summary>
        /// Decide whether or not the given model should be included by the filter
        /// </summary>
        /// <remarks>Filters is guaranteed to be non-empty when this method is called</remarks>
        /// <param name="modelObject">The model object under consideration</param>
        /// <returns>True if the object is included by the filter</returns>
        abstract public bool FilterObject(object modelObject);
    }

    /// <summary>
    /// A CompositeAllFilter joins several other filters together.
    /// A model object must satisfy all filters to be included.
    /// If there are no filters, all model objects are included
    /// </summary>
    public class CompositeAllFilter : CompositeFilter {

        /// <summary>
        /// Create a filter
        /// </summary>
        /// <param name="filters"></param>
        public CompositeAllFilter(List<IModelFilter> filters)
            : base(filters) {
        }

        /// <summary>
        /// Decide whether or not the given model should be included by the filter
        /// </summary>
        /// <remarks>Filters is guaranteed to be non-empty when this method is called</remarks>
        /// <param name="modelObject">The model object under consideration</param>
        /// <returns>True if the object is included by the filter</returns>
        override public bool FilterObject(object modelObject) {
            foreach (IModelFilter filter in Filters)
                if (!filter.Filter(modelObject))
                    return false;

            return true;
        }
    }

    /// <summary>
    /// A CompositeAllFilter joins several other filters together.
    /// A model object must only satisfy one of the filters to be included.
    /// If there are no filters, all model objects are included
    /// </summary>
    public class CompositeAnyFilter : CompositeFilter {

        /// <summary>
        /// Create a filter from the given filters
        /// </summary>
        /// <param name="filters"></param>
        public CompositeAnyFilter(List<IModelFilter> filters)
            : base(filters) {
        }

        /// <summary>
        /// Decide whether or not the given model should be included by the filter
        /// </summary>
        /// <remarks>Filters is guaranteed to be non-empty when this method is called</remarks>
        /// <param name="modelObject">The model object under consideration</param>
        /// <returns>True if the object is included by the filter</returns>
        override public bool FilterObject(object modelObject) {
            foreach (IModelFilter filter in Filters)
                if (filter.Filter(modelObject))
                    return true;

            return false;
        }
    }

    /// <summary>
    /// Instances of this class extract a value from the model object
    /// and compare that value to a list of fixed values. The model
    /// object is included if the extracted value is in the list
    /// </summary>
    /// <remarks>If there is no delegate installed or there are
    /// no values to match, no model objects will be matched</remarks>
    public class OneOfFilter : IModelFilter {

        /// <summary>
        /// Create a filter that will use the given delegate to extract values
        /// </summary>
        /// <param name="valueGetter"></param>
        public OneOfFilter(AspectGetterDelegate valueGetter) :
            this(valueGetter, new ArrayList()) {
        }

        /// <summary>
        /// Create a filter that will extract values using the given delegate
        /// and compare them to the values in the given list.
        /// </summary>
        /// <param name="valueGetter"></param>
        /// <param name="possibleValues"></param>
        public OneOfFilter(AspectGetterDelegate valueGetter, ICollection possibleValues) {
            ValueGetter = valueGetter;
            PossibleValues = new ArrayList(possibleValues);
        }

        /// <summary>
        /// Gets or sets the delegate that will be used to extract values
        /// from model objects
        /// </summary>
        virtual public AspectGetterDelegate ValueGetter {
            get { return valueGetter; }
            set { valueGetter = value; }
        }
        private AspectGetterDelegate valueGetter;

        /// <summary>
        /// Gets or sets the list of values that the value extracted from
        /// the model object must match in order to be included.
        /// </summary>
        virtual public IList PossibleValues {
            get { return possibleValues; }
            set { possibleValues = value; }
        }
        private IList possibleValues;

        /// <summary>
        /// Should the given model object be included?
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public virtual bool Filter(object modelObject) {
            if (ValueGetter == null || PossibleValues == null || PossibleValues.Count == 0)
                return false;

            object result = ValueGetter(modelObject);
            if (result is string || !(result is IEnumerable enumerable))
                return DoesValueMatch(result);

            foreach (object x in enumerable) {
                if (DoesValueMatch(x))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Decides if the given property is a match for the values in the PossibleValues collection
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool DoesValueMatch(object result) {
            return PossibleValues.Contains(result);
        }
    }

    /// <summary>
    /// Instances of this class match a property of a model objects against
    /// a list of bit flags. The property should be an xor-ed collection
    /// of bits flags.
    /// </summary>
    /// <remarks>Both the property compared and the list of possible values 
    /// must be convertible to ulongs.</remarks>
    public class FlagBitSetFilter : OneOfFilter {

        /// <summary>
        /// Create an instance
        /// </summary>
        /// <param name="valueGetter"></param>
        /// <param name="possibleValues"></param>
        public FlagBitSetFilter(AspectGetterDelegate valueGetter, ICollection possibleValues) : base(valueGetter, possibleValues) {
            ConvertPossibleValues();
        }

        /// <summary>
        /// Gets or sets the collection of values that will be matched.
        /// These must be ulongs (or convertible to ulongs).
        /// </summary>
        public override IList PossibleValues {
            get { return base.PossibleValues; }
            set {
                base.PossibleValues = value;
                ConvertPossibleValues();
            }
        }

        private void ConvertPossibleValues() {
            possibleValuesAsUlongs = new List<UInt64>();
            foreach (object x in PossibleValues)
                possibleValuesAsUlongs.Add(Convert.ToUInt64(x));
        }

        /// <summary>
        /// Decides if the given property is a match for the values in the PossibleValues collection
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected override bool DoesValueMatch(object result) {
            try {
                UInt64 value = Convert.ToUInt64(result);
                foreach (ulong flag in possibleValuesAsUlongs) {
                    if ((value & flag) == flag)
                        return true;
                }
                return false;
            }
            catch (InvalidCastException) {
                return false;
            }
            catch (FormatException) {
                return false;
            }
        }

        private List<UInt64> possibleValuesAsUlongs = new();
    }

    /// <summary>
    /// Base class for whole list filters
    /// </summary>
    public class AbstractListFilter : IListFilter
    {
        /// <summary>
        /// Return a subset of the given list of model objects as the new
        /// contents of the ObjectListView
        /// </summary>
        /// <param name="modelObjects">The collection of model objects that the list will possibly display</param>
        /// <returns>The filtered collection that holds the model objects that will be displayed.</returns>
        virtual public IEnumerable Filter(IEnumerable modelObjects) {
            return modelObjects;
        }
    }

    /// <summary>
    /// Instance of this class implement delegate based whole list filtering
    /// </summary>
    public class ListFilter : AbstractListFilter
    {
        /// <summary>
        /// A delegate that filters on a whole list
        /// </summary>
        /// <param name="rowObjects"></param>
        /// <returns></returns>
        public delegate IEnumerable ListFilterDelegate(IEnumerable rowObjects);

        /// <summary>
        /// Create a ListFilter
        /// </summary>
        /// <param name="function"></param>
        public ListFilter(ListFilterDelegate function) {
            Function = function;
        }

        /// <summary>
        /// Gets or sets the delegate that will filter the list
        /// </summary>
        public ListFilterDelegate Function {
            get { return function; }
            set { function = value; }
        }
        private ListFilterDelegate function;

        /// <summary>
        /// Do the actual work of filtering
        /// </summary>
        /// <param name="modelObjects"></param>
        /// <returns></returns>
        public override IEnumerable Filter(IEnumerable modelObjects) {
            if (Function == null)
                return modelObjects;

            return Function(modelObjects);
        }
    }

    /// <summary>
    /// Filter the list so only the last N entries are displayed
    /// </summary>
    public class TailFilter : AbstractListFilter
    {
        /// <summary>
        /// Create a no-op tail filter
        /// </summary>
        public TailFilter() {
        }

        /// <summary>
        /// Create a filter that includes on the last N model objects
        /// </summary>
        /// <param name="numberOfObjects"></param>
        public TailFilter(int numberOfObjects) {
            Count = numberOfObjects;
        }

        /// <summary>
        /// Gets or sets the number of model objects that will be 
        /// returned from the tail of the list
        /// </summary>
        public int Count {
            get { return count; }
            set { count = value; }
        }
        private int count;

        /// <summary>
        /// Return the last N subset of the model objects
        /// </summary>
        /// <param name="modelObjects"></param>
        /// <returns></returns>
        public override IEnumerable Filter(IEnumerable modelObjects) {
            if (Count <= 0)
                return modelObjects;

            ArrayList list = ObjectListView.EnumerableToArray(modelObjects, false);

            if (Count > list.Count)
                return list;

            object[] tail = new object[Count];
            list.CopyTo(list.Count - Count, tail, 0, Count);
            return new ArrayList(tail);
        }
    }
}