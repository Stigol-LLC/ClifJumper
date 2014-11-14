#region Usings

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

#endregion

namespace UIEditor.Util {
    public class Reflection {
        #region Type

        public struct Property {
            #region Properties

            public string Name;
            public object Value;

            #endregion
        }

        #endregion

        #region Properties

        private readonly object _target;
        public object Target {
            get { return _target; }
        }
        private MemberInfo _currentMemberInfo;

        #endregion

        #region Constructor

        private Reflection( object target ) {
            if ( target == null ) {
                throw new ArgumentNullException( "target" );
            }
            _target = target;
        }

        #endregion

        #region Methods

		public MemberInfo CurrentMemberInfo{
			get{ return _currentMemberInfo; }

		}
        public static FieldInfo FindField( Type type, string name ) {
            if ( type == null ||
                 type == typeof (object) ) {
                return null;
            }
            return
                    type.GetField(
                            name,
                            BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public |
                            BindingFlags.NonPublic ) ?? FindField( type.BaseType, name );
        }


        public static IEnumerable FindPublicMethods(object target)
        {
            Type type = target.GetType();

            MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly  );

            foreach (MethodInfo methodInfo in methodInfos)
            {
                yield return methodInfo;

            }
        }

		public static IEnumerable FindFields( object target ) {
			Type type = target.GetType();

			FieldInfo[] fieldInfos = type.GetFields (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public |
			                                         BindingFlags.NonPublic);

			foreach ( FieldInfo propertyInfo in fieldInfos ) {
				yield return propertyInfo;

			}
		}

        public static IEnumerable FindOnlyPublicFields(object target)
        {
            Type type = target.GetType();

            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);

            foreach (FieldInfo propertyInfo in fieldInfos)
            {
                yield return propertyInfo;

            }
        }

        public static IEnumerable FindProperties( object target ) {
            Type type = target.GetType();
            PropertyInfo[] propertyInfos =
                    type.GetProperties( BindingFlags.Instance | BindingFlags.Public )
                        .Where( p => p.CanRead && p.CanWrite )
                        .ToArray();
            foreach ( PropertyInfo propertyInfo in propertyInfos ) {
                //object value = null;
//                try {
//                    value = propertyInfo.GetValue( target, null );
//                } catch ( Exception exception ) {
//                    Debug.LogError( exception.Message );
//                }
				yield return propertyInfo;
//                yield return new Property {
//                    Name = propertyInfo.Name,
//                    Value = value
//                };
            }
        }

		public static IEnumerable FindPropertiesWithSetters( object target ) {
			Type type = target.GetType();
			PropertyInfo[] propertyInfos =
				type.GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.SetField | BindingFlags.DeclaredOnly )
					.Where( p => p.CanRead && p.CanWrite )
					.ToArray();
			foreach ( PropertyInfo propertyInfo in propertyInfos ) {
		
				yield return propertyInfo;

			}
		}

        public static PropertyInfo FindProperty( Type type, string name ) {
            if ( type == null ||
                 type == typeof (object) ) {
                return null;
            }
            return
                    type.GetProperty(
                            name,
                            BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public |
                            BindingFlags.NonPublic ) ?? FindProperty( type.BaseType, name );
        }

        public static Reflection For( object targetInstance ) {
            return new Reflection( targetInstance );
        }

        public static bool IsPrimitiveOrStruct( Type type ) {
            return ( type.IsPrimitive || type == typeof (string) || type.IsValueType );
        }

        public Reflection FieldOrProperty( string fieldName ) {

            _currentMemberInfo = FindField( _target.GetType(), fieldName );
            if ( _currentMemberInfo == null ) {
                _currentMemberInfo = FindProperty( _target.GetType(), fieldName );
                if ( _currentMemberInfo == null ) {
					Debug.LogError("Object has no property" + fieldName);
					return null;
                   // throw new NullReferenceException(
                   //         string.Format(
                   //                 "Sorry, There is no such field or property = {0} in class type {1}",
                   //                 fieldName,
                   //                 _target.GetType().FullName ) );
                }
            }
            return this;
        }

        public Reflection GetField( string fieldName ) {
            _currentMemberInfo = FindField( _target.GetType(), fieldName );
            if ( _currentMemberInfo == null ) {
                throw new NullReferenceException(
                        string.Format(
                                "Sorry, There is no such field = {0} in class type {1}",
                                fieldName,
                                _target.GetType().FullName ) );
            }
            return this;
        }

        public Reflection GetProperty( string propertyName ) {
            _currentMemberInfo = FindProperty( _target.GetType(), propertyName );
            if ( _currentMemberInfo == null ) {
                throw new NullReferenceException(
                        string.Format(
                                "Sorry, There is no such property = {0} in class type {1}",
                                propertyName,
                                _target.GetType().FullName ) );
            }
            return this;
        }

        public T GetTarget<T>() {
            return (T) _target;
        }

        public object GetValue() {
            Type type = _currentMemberInfo.GetType();
            if ( type.IsSubclassOf( typeof (PropertyInfo) ) ) {
                return ( (PropertyInfo) _currentMemberInfo ).GetValue( _target, null );
            }
            if ( type.IsSubclassOf( typeof (FieldInfo) ) ) {
                return ( (FieldInfo) _currentMemberInfo ).GetValue( _target );
            }
            throw new NotSupportedException( string.Format( "Unsupported type of modified memeber. {0}", type.Name ) );
        }
		public bool TrySetValue(object value, object[] index = null){
			if ( _currentMemberInfo == null ) {
				throw new NullReferenceException( "Modified member is null." );
			}
			Type type = _currentMemberInfo.GetType();
			if ( type.IsSubclassOf( typeof (PropertyInfo) ) ) {
				PropertyInfo propertyInfo = (PropertyInfo) _currentMemberInfo;
				try{
					propertyInfo.SetValue( _target, value, index );
				}catch(Exception e){
					Debug.LogError( "propertyInfo \"" + propertyInfo.Name + "\" _target " +_target.ToString() + " = " + value.ToString() + "" + e);
				};
				return true;
			}
			return false;
		}
        public Reflection SetValue( object value, Func<Type, object> converter, object[] index = null ) {
            if ( _currentMemberInfo == null ) {
                throw new NullReferenceException( "Modified member is null." );
            }
            Type type = _currentMemberInfo.GetType();
            if ( type.IsSubclassOf( typeof (PropertyInfo) ) ) {
                PropertyInfo propertyInfo = (PropertyInfo) _currentMemberInfo;
                value = propertyInfo.PropertyType.Namespace.Contains( "UnityEngine" )
                                ? converter( propertyInfo.PropertyType )
                                : Convert.ChangeType(
                                        value,
                                        propertyInfo.PropertyType,
                                        CultureInfo.InvariantCulture.NumberFormat );
                propertyInfo.SetValue( _target, value, index );
            } else {
                if ( !type.IsSubclassOf( typeof (FieldInfo) ) ) {
                    throw new NotSupportedException(
                            string.Format( "Unsupported type of modified memeber. {0}", type.Name ) );
                }
                FieldInfo fieldInfo = (FieldInfo) _currentMemberInfo;
                value = converter != null
                                ? converter( fieldInfo.FieldType )
                                : Convert.ChangeType(
                                        value,
                                        fieldInfo.FieldType,
                                        CultureInfo.InvariantCulture.NumberFormat );
                fieldInfo.SetValue( _target, value );
            }
            return this;
        }


       

        #endregion
    }
}
