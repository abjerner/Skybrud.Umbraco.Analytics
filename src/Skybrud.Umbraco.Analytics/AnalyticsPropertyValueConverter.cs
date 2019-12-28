using System;
using Skybrud.Umbraco.Analytics.Models.Selection;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Skybrud.Umbraco.Analytics {

    public class AnalyticsPropertyValueConverter : IPropertyValueConverter {

        public bool IsConverter(IPublishedPropertyType propertyType) {
            return propertyType.EditorAlias == "Skybrud.Umbraco.Analytics.Profile";
        }

        public bool? IsValue(object value, PropertyValueLevel level) {
            // TODO: What are we supposed to return here?
            return null;
        }

        public Type GetPropertyValueType(IPublishedPropertyType propertyType) {
            return typeof(AnalyticsProfileSelection);
        }

        public PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
            // TODO: Return the correct cache level
            return PropertyCacheLevel.None;
        }

        public object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview) {
            return source;
        }

        public object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            try {
                return AnalyticsProfileSelection.Deserialize(inter + "");
            } catch (Exception) {
                return AnalyticsProfileSelection.Deserialize("{}");
            }
        }

        public object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            return null;
        }

    }

}