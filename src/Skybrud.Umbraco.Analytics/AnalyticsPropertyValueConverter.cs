using System;
using Skybrud.Umbraco.Analytics.Models.Selection;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Skybrud.Umbraco.Analytics {

    public class AnalyticsPropertyValueConverter : IPropertyValueConverter {

        public bool IsConverter(PublishedPropertyType propertyType) {
            return propertyType.EditorAlias == "Skybrud.Umbraco.Analytics.Profile";
        }

        public bool? IsValue(object value, PropertyValueLevel level) {
            // TODO: What are we supposed to return here?
            return null;
        }

        public Type GetPropertyValueType(PublishedPropertyType propertyType) {
            return typeof(AnalyticsProfileSelection);
        }

        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType) {
            // TODO: Return the correct cache level
            return PropertyCacheLevel.None;
        }

        public object ConvertSourceToIntermediate(IPublishedElement owner, PublishedPropertyType propertyType, object source, bool preview) {
            return source;
        }

        public object ConvertIntermediateToObject(IPublishedElement owner, PublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            try {
                return AnalyticsProfileSelection.Deserialize(inter + "");
            } catch (Exception) {
                return AnalyticsProfileSelection.Deserialize("{}");
            }
        }

        public object ConvertIntermediateToXPath(IPublishedElement owner, PublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            return null;
        }

    }

}