// Definitions from Node.JS js_native_api.h and js_native_api_types.h

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security;

namespace Microsoft.JavaScript.NodeApi;

public static partial class JSNativeApi
{
    // Node-API Interop definitions and functions.
    [SuppressUnmanagedCodeSecurity]
    public unsafe partial class Interop
    {
        private static bool s_initialized;

        public static void Initialize()
        {
            if (s_initialized) return;
            s_initialized = true;

            // Node APIs are all imported from the main `node` executable. Overriding the import
            // resolution is more efficient and avoids issues with library search paths and
            // differences in the name of the executable.
            NativeLibrary.SetDllImportResolver(
              typeof(JSNativeApi).Assembly,
              (libraryName, _, _) =>
              {
                  return libraryName switch
                  {
                      nameof(DotNetHost.HostFxr) => DotNetHost.HostFxr.Handle,
                      nameof(NodeApi) => NativeLibrary.GetMainProgramHandle(),
                      _ => default,
                  };
              });
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate napi_value napi_register_module_v1(napi_env env, napi_value exports);

        public static readonly nuint NAPI_AUTO_LENGTH = nuint.MaxValue;

        //===========================================================================
        // Specialized pointer types
        //===========================================================================

        public record struct napi_env(nint Handle)
        {
            public bool IsNull => Handle == nint.Zero;
            public static napi_env Null => new(nint.Zero);
        }
        public record struct napi_value(nint Handle)
        {
            public static napi_value Null => new(nint.Zero);
            public bool IsNull => Handle == nint.Zero;
        }
        public record struct napi_ref(nint Handle);
        public record struct napi_handle_scope(nint Handle);
        public record struct napi_escapable_handle_scope(nint Handle);
        public record struct napi_callback_info(nint Handle);
        public record struct napi_deferred(nint Handle);

        //===========================================================================
        // Enum types
        //===========================================================================

        public enum napi_property_attributes : int
        {
            napi_default = 0,
            napi_writable = 1 << 0,
            napi_enumerable = 1 << 1,
            napi_configurable = 1 << 2,

            // Used with napi_define_class to distinguish static properties
            // from instance properties. Ignored by napi_define_properties.
            napi_static = 1 << 10,

            // Default for class methods.
            napi_default_method = napi_writable | napi_configurable,

            // Default for object properties, like in JS obj[prop].
            napi_default_jsproperty = napi_writable | napi_enumerable | napi_configurable,
        }

        public enum napi_valuetype : int
        {
            // ES6 types (corresponds to typeof)
            napi_undefined,
            napi_null,
            napi_boolean,
            napi_number,
            napi_string,
            napi_symbol,
            napi_object,
            napi_function,
            napi_external,
            napi_bigint,
        }

        public enum napi_typedarray_type : int
        {
            napi_int8_array,
            napi_uint8_array,
            napi_uint8_clamped_array,
            napi_int16_array,
            napi_uint16_array,
            napi_int32_array,
            napi_uint32_array,
            napi_float32_array,
            napi_float64_array,
            napi_bigint64_array,
            napi_biguint64_array,
        }

        public enum napi_status : int
        {
            napi_ok,
            napi_invalid_arg,
            napi_object_expected,
            napi_string_expected,
            napi_name_expected,
            napi_function_expected,
            napi_number_expected,
            napi_boolean_expected,
            napi_array_expected,
            napi_generic_failure,
            napi_pending_exception,
            napi_cancelled,
            napi_escape_called_twice,
            napi_handle_scope_mismatch,
            napi_callback_scope_mismatch,
            napi_queue_full,
            napi_closing,
            napi_bigint_expected,
            napi_date_expected,
            napi_arraybuffer_expected,
            napi_detachable_arraybuffer_expected,
            napi_would_deadlock,
        }

        public unsafe struct napi_callback
        {
            public delegate* unmanaged[Cdecl]<napi_env, napi_callback_info, napi_value> Handle;

            public napi_callback(delegate* unmanaged[Cdecl]<napi_env, napi_callback_info, napi_value> handle) =>
              Handle = handle;
        }

        public unsafe struct napi_finalize
        {
            public delegate* unmanaged[Cdecl]<napi_env, nint, nint, void> Handle;

            public napi_finalize(delegate* unmanaged[Cdecl]<napi_env, nint, nint, void> handle) =>
              Handle = handle;
        }

        public unsafe struct napi_property_descriptor
        {
            // One of utf8name or name should be NULL.
            public nint utf8name;
            public napi_value name;

            public napi_callback method;
            public napi_callback getter;
            public napi_callback setter;
            public napi_value value;

            public napi_property_attributes attributes;
            public nint data;
        }

        public struct napi_extended_error_info
        {
            public byte* error_message;
            public nint engine_reserved;
            public uint engine_error_code;
            public napi_status error_code;
        }

        public enum napi_key_collection_mode : int
        {
            napi_key_include_prototypes,
            napi_key_own_only,
        }

        [Flags]
        public enum napi_key_filter : int
        {
            napi_key_all_properties = 0,
            napi_key_writable = 1 << 0,
            napi_key_enumerable = 1 << 1,
            napi_key_configurable = 1 << 2,
            napi_key_skip_strings = 1 << 3,
            napi_key_skip_symbols = 1 << 4,
        }

        public enum napi_key_conversion : int
        {
            napi_key_keep_numbers,
            napi_key_numbers_to_strings,
        }

        public struct napi_type_tag
        {
            public ulong lower;
            public ulong upper;
        }

        public readonly struct c_bool
        {
            private readonly byte _value;

            public c_bool(bool value) => _value = (byte)(value ? 1 : 0);

            public static implicit operator c_bool(bool value) => new(value);
            public static explicit operator bool(c_bool value) => value._value != 0;

            public static readonly c_bool True = new(true);
            public static readonly c_bool False = new(false);
        }

        private nint _libraryHandle;

        private nint _napi_get_last_error_info;
        private nint _napi_get_undefined;
        private nint _napi_get_null;
        private nint _napi_get_global;
        private nint _napi_get_boolean;
        private nint _napi_create_object;
        private nint _napi_create_array;
        private nint _napi_create_array_with_length;
        private nint _napi_create_double;
        private nint _napi_create_int32;
        private nint _napi_create_uint32;
        private nint _napi_create_int64;
        private nint _napi_create_string_latin1;
        private nint _napi_create_string_utf8;
        private nint _napi_create_string_utf16;
        private nint _napi_create_symbol;
        private nint _node_api_symbol_for;
        private nint _napi_create_function;
        private nint _napi_create_error;
        private nint _napi_create_type_error;
        private nint _napi_create_range_error;
        private nint _node_api_create_syntax_error;
        private nint _napi_typeof;
        private nint _napi_get_value_double;
        private nint _napi_get_value_int32;
        private nint _napi_get_value_uint32;
        private nint _napi_get_value_int64;
        private nint _napi_get_value_bool;
        private nint _napi_get_value_string_latin1;
        private nint _napi_get_value_string_utf8;
        private nint _napi_get_value_string_utf16;
        private nint _napi_coerce_to_bool;
        private nint _napi_coerce_to_number;
        private nint _napi_coerce_to_object;
        private nint _napi_coerce_to_string;
        private nint _napi_get_prototype;
        private nint _napi_get_property_names;
        private nint _napi_set_property;
        private nint _napi_has_property;
        private nint _napi_get_property;
        private nint _napi_delete_property;
        private nint _napi_has_own_property;
        private nint _napi_set_named_property;
        private nint _napi_has_named_property;
        private nint _napi_get_named_property;
        private nint _napi_set_element;
        private nint _napi_has_element;
        private nint _napi_get_element;
        private nint _napi_delete_element;
        private nint _napi_define_properties;
        private nint _napi_is_array;
        private nint _napi_get_array_length;
        private nint _napi_strict_equals;
        private nint _napi_call_function;
        private nint _napi_new_instance;
        private nint _napi_instanceof;
        private nint _napi_get_cb_info;
        private nint _napi_get_new_target;
        private nint _napi_define_class;
        private nint _napi_wrap;
        private nint _napi_unwrap;
        private nint _napi_remove_wrap;
        private nint _napi_create_external;
        private nint _napi_get_value_external;
        private nint _napi_create_reference;
        private nint _napi_delete_reference;
        private nint _napi_reference_ref;
        private nint _napi_reference_unref;
        private nint _napi_get_reference_value;
        private nint _napi_open_handle_scope;
        private nint _napi_close_handle_scope;
        private nint _napi_open_escapable_handle_scope;
        private nint _napi_close_escapable_handle_scope;
        private nint _napi_escape_handle;
        private nint _napi_throw;
        private nint _napi_throw_error;
        private nint _napi_throw_type_error;
        private nint _napi_throw_range_error;
        private nint _node_api_throw_syntax_error;
        private nint _napi_is_error;
        private nint _napi_is_exception_pending;
        private nint _napi_get_and_clear_last_exception;
        private nint _napi_is_arraybuffer;
        private nint _napi_create_arraybuffer;
        private nint _napi_create_external_arraybuffer;
        private nint _napi_get_arraybuffer_info;
        private nint _napi_is_typedarray;
        private nint _napi_create_typedarray;
        private nint _napi_get_typedarray_info;
        private nint _napi_create_dataview;
        private nint _napi_is_dataview;
        private nint _napi_get_dataview_info;
        private nint _napi_get_version;
        private nint _napi_create_promise;
        private nint _napi_resolve_deferred;
        private nint _napi_reject_deferred;
        private nint _napi_is_promise;
        private nint _napi_run_script;
        private nint _napi_adjust_external_memory;
        private nint _napi_create_date;
        private nint _napi_is_date;
        private nint _napi_get_date_value;
        private nint _napi_add_finalizer;
        private nint _napi_create_bigint_int64;
        private nint _napi_create_bigint_uint64;
        private nint _napi_create_bigint_words;
        private nint _napi_get_value_bigint_int64;
        private nint _napi_get_value_bigint_uint64;
        private nint _napi_get_value_bigint_words;
        private nint _napi_get_all_property_names;
        private nint _napi_set_instance_data;
        private nint _napi_get_instance_data;
        private nint _napi_detach_arraybuffer;
        private nint _napi_is_detached_arraybuffer;
        private nint _napi_type_tag_object;
        private nint _napi_check_object_type_tag;
        private nint _napi_object_freeze;
        private nint _napi_object_seal;

        internal napi_status napi_get_last_error_info(napi_env env, out nint result)
            => CallInterop(ref _napi_get_last_error_info, env, out result);

        internal napi_status napi_get_undefined(napi_env env, out napi_value result)
            => CallInterop(ref _napi_get_undefined, env, out result);

        internal napi_status napi_get_null(napi_env env, out napi_value result)
            => CallInterop(ref _napi_get_null, env, out result);

        internal napi_status napi_get_global(napi_env env, out napi_value result)
            => CallInterop(ref _napi_get_global, env, out result);

        internal napi_status napi_get_boolean(napi_env env, c_bool value, out napi_value result)
            => CallInterop(ref _napi_get_boolean, env, value, out result);

        internal napi_status napi_create_object(napi_env env, out napi_value result)
            => CallInterop(ref _napi_create_object, env, out result);

        internal napi_status napi_create_array(napi_env env, out napi_value result)
            => CallInterop(ref _napi_create_array, env, out result);

        internal napi_status napi_create_array_with_length(
            napi_env env, nuint length, out napi_value result)
            => CallInterop(ref _napi_create_array_with_length, env, length, out result);

        internal napi_status napi_create_double(napi_env env, double value, out napi_value result)
            => CallInterop(ref _napi_create_double, env, value, out result);

        internal napi_status napi_create_int32(napi_env env, int value, out napi_value result)
            => CallInterop(ref _napi_create_int32, env, value, out result);

        internal napi_status napi_create_uint32(napi_env env, uint value, out napi_value result)
            => CallInterop(ref _napi_create_uint32, env, value, out result);

        internal napi_status napi_create_int64(napi_env env, long value, out napi_value result)
            => CallInterop(ref _napi_create_int64, env, value, out result);

        internal napi_status napi_create_string_latin1(
            napi_env env, nint str, nuint length, out napi_value result)
            => CallInterop(ref _napi_create_string_latin1, env, str, length, out result);

        internal napi_status napi_create_string_utf8(
            napi_env env, nint str, nuint length, out napi_value result)
            => CallInterop(ref _napi_create_string_utf8, env, str, length, out result);

        internal napi_status napi_create_string_utf16(
            napi_env env, nint str, nuint length, out napi_value result)
            => CallInterop(ref _napi_create_string_utf16, env, str, length, out result);

        internal napi_status napi_create_symbol(
            napi_env env, napi_value description, out napi_value result)
            => CallInterop(ref _napi_create_symbol, env, description, out result);

        internal napi_status node_api_symbol_for(
            napi_env env, nint utf8name, nuint length, out napi_value result)
            => CallInterop(ref _node_api_symbol_for, env, utf8name, length, out result);

        internal napi_status napi_create_function(napi_env env, nint utf8name, nuint length,
            napi_callback cb, nint data, out napi_value result)
            => CallInterop(ref _napi_create_function, env, utf8name, length, cb, data, out result);

        internal napi_status napi_create_error(
            napi_env env, napi_value code, napi_value msg, out napi_value result)
            => CallInterop(ref _napi_create_error, env, code, msg, out result);

        internal napi_status napi_create_type_error(
            napi_env env, napi_value code, napi_value msg, out napi_value result)
            => CallInterop(ref _napi_create_type_error, env, code, msg, out result);

        internal napi_status napi_create_range_error(
            napi_env env, napi_value code, napi_value msg, out napi_value result)
            => CallInterop(ref _napi_create_range_error, env, code, msg, out result);

        internal napi_status node_api_create_syntax_error(
            napi_env env, napi_value code, napi_value msg, out napi_value result)
            => CallInterop(ref _node_api_create_syntax_error, env, code, msg, out result);

        internal napi_status napi_typeof(napi_env env, napi_value value, out napi_valuetype result)
            => CallInterop(ref _napi_typeof, env, value, out result);

        internal napi_status napi_get_value_double(
            napi_env env, napi_value value, out double result)
            => CallInterop(ref _napi_get_value_double, env, value, out result);

        internal napi_status napi_get_value_int32(napi_env env, napi_value value, out int result)
            => CallInterop(ref _napi_get_value_int32, env, value, out result);

        internal napi_status napi_get_value_uint32(napi_env env, napi_value value, out uint result)
            => CallInterop(ref _napi_get_value_uint32, env, value, out result);

        internal napi_status napi_get_value_int64(napi_env env, napi_value value, out long result)
            => CallInterop(ref _napi_get_value_int64, env, value, out result);

        internal napi_status napi_get_value_bool(napi_env env, napi_value value, out c_bool result)
            => CallInterop(ref _napi_get_value_bool, env, value, out result);

        internal napi_status napi_get_value_string_latin1(
            napi_env env, napi_value value, nint buf, nuint bufsize, out nuint result)
            => CallInterop(
                ref _napi_get_value_string_latin1, env, value, buf, bufsize, out result);

        internal napi_status napi_get_value_string_utf8(
            napi_env env, napi_value value, nint buf, nuint bufsize, out nuint result)
            => CallInterop(ref _napi_get_value_string_utf8, env, value, buf, bufsize, out result);

        internal napi_status napi_get_value_string_utf16(
            napi_env env, napi_value value, nint buf, nuint bufsize, out nuint result)
            => CallInterop(ref _napi_get_value_string_utf16, env, value, buf, bufsize, out result);

        internal napi_status napi_coerce_to_bool(
            napi_env env, napi_value value, out napi_value result)
            => CallInterop(ref _napi_coerce_to_bool, env, value, out result);

        internal napi_status napi_coerce_to_number(
            napi_env env, napi_value value, out napi_value result)
            => CallInterop(ref _napi_coerce_to_number, env, value, out result);

        internal napi_status napi_coerce_to_object(
            napi_env env, napi_value value, out napi_value result)
            => CallInterop(ref _napi_coerce_to_object, env, value, out result);

        internal napi_status napi_coerce_to_string(
            napi_env env, napi_value value, out napi_value result)
            => CallInterop(ref _napi_coerce_to_string, env, value, out result);

        internal napi_status napi_get_prototype(
            napi_env env, napi_value js_object, out napi_value result)
            => CallInterop(ref _napi_get_prototype, env, js_object, out result);

        internal napi_status napi_get_property_names(
            napi_env env, napi_value js_object, out napi_value result)
            => CallInterop(ref _napi_get_property_names, env, js_object, out result);

        internal napi_status napi_set_property(
            napi_env env, napi_value js_object, napi_value key, napi_value value)
            => CallInterop(ref _napi_set_property, env, js_object, key, value);

        internal napi_status napi_has_property(
            napi_env env, napi_value js_object, napi_value key, out c_bool result)
            => CallInterop(ref _napi_has_property, env, js_object, key, out result);

        internal napi_status napi_get_property(
            napi_env env, napi_value js_object, napi_value key, out napi_value result)
            => CallInterop(ref _napi_get_property, env, js_object, key, out result);

        internal napi_status napi_delete_property(
            napi_env env, napi_value js_object, napi_value key, out c_bool result)
            => CallInterop(ref _napi_delete_property, env, js_object, key, out result);

        internal napi_status napi_has_own_property(
            napi_env env, napi_value js_object, napi_value key, out c_bool result)
            => CallInterop(ref _napi_has_own_property, env, js_object, key, out result);

        internal napi_status napi_set_named_property(
            napi_env env, napi_value js_object, nint utf8name, napi_value value)
            => CallInterop(ref _napi_set_named_property, env, js_object, utf8name, value);

        internal napi_status napi_has_named_property(
            napi_env env, napi_value js_object, nint utf8name, out c_bool result)
            => CallInterop(ref _napi_has_named_property, env, js_object, utf8name, out result);

        internal napi_status napi_get_named_property(
            napi_env env, napi_value js_object, nint utf8name, out napi_value result)
            => CallInterop(ref _napi_get_named_property, env, js_object, utf8name, out result);

        internal napi_status napi_set_element(
            napi_env env, napi_value js_object, uint index, napi_value value)
            => CallInterop(ref _napi_set_element, env, js_object, index, value);

        internal napi_status napi_has_element(
            napi_env env, napi_value js_object, uint index, out c_bool result)
            => CallInterop(ref _napi_has_element, env, js_object, index, out result);

        internal napi_status napi_get_element(
            napi_env env, napi_value js_object, uint index, out napi_value result)
            => CallInterop(ref _napi_get_element, env, js_object, index, out result);

        internal napi_status napi_delete_element(
            napi_env env, napi_value js_object, uint index, out c_bool result)
            => CallInterop(ref _napi_delete_element, env, js_object, index, out result);

        internal napi_status napi_define_properties(
            napi_env env, napi_value js_object, nuint property_count, nint properties)
            => CallInterop(
                ref _napi_define_properties, env, js_object, property_count, properties);

        internal napi_status napi_is_array(napi_env env, napi_value value, out c_bool result)
            => CallInterop(ref _napi_is_array, env, value, out result);

        internal napi_status napi_get_array_length(napi_env env, napi_value value, out uint result)
            => CallInterop(ref _napi_get_array_length, env, value, out result);

        internal napi_status napi_strict_equals(
            napi_env env, napi_value lhs, napi_value rhs, out c_bool result)
            => CallInterop(ref _napi_strict_equals, env, lhs, rhs, out result);

        internal napi_status napi_call_function(napi_env env,
            napi_value recv, napi_value func, nuint argc, nint argv, out napi_value result)
            => CallInterop(ref _napi_call_function, env, recv, func, argc, argv, out result);

        internal napi_status napi_new_instance(napi_env env,
            napi_value constructor, nuint argc, nint argv, out napi_value result)
            => CallInterop(ref _napi_new_instance, env, constructor, argc, argv, out result);

        internal napi_status napi_instanceof(
            napi_env env, napi_value js_object, napi_value constructor, out c_bool result)
            => CallInterop(ref _napi_instanceof, env, js_object, constructor, out result);

        internal napi_status napi_get_cb_info(
            napi_env env,              // [in] NAPI environment handle
            napi_callback_info cbinfo, // [in] Opaque callback-info handle
            nuint* argc,               // [in-out] Specifies the size of the provided argv array
                                       // and receives the actual count of args.
            napi_value* argv,          // [out] Array of values
            napi_value* this_arg,      // [out] Receives the JS 'this' arg for the call
            nint* data)                // [out] Receives the data pointer for the callback.
        {
            nint funcHandle = (_napi_get_cb_info != nint.Zero)
                ? _napi_get_cb_info
                : _napi_get_cb_info = GetExport(nameof(napi_get_cb_info));
            var funcDelegate = (delegate* unmanaged[Cdecl]<napi_env, napi_callback_info,
                nuint*, napi_value*, napi_value*, nint*, napi_status>)funcHandle;
            return funcDelegate(env, cbinfo, argc, argv, this_arg, data);
        }

        internal napi_status napi_get_new_target(
            napi_env env, napi_callback_info cbinfo, out napi_value result)
            => CallInterop(ref _napi_get_new_target, env, cbinfo, out result);

        internal napi_status napi_define_class(
            napi_env env,
            byte* utf8name,
            nuint length,
            napi_callback constructor,
            nint data,
            nuint property_count,
            napi_property_descriptor* properties,
            out napi_value result)
        {
            nint funcHandle = (_napi_define_class != nint.Zero)
                ? _napi_define_class
                : _napi_define_class = GetExport(nameof(napi_define_class));
            var funcDelegate = (delegate* unmanaged[Cdecl]<napi_env, byte*, nuint, napi_callback,
                nint, nuint, napi_property_descriptor*, napi_value*, napi_status>)funcHandle;
            fixed (napi_value* native_result = &result)
            {
                return funcDelegate(env, utf8name, length, constructor, data,
                    property_count, properties, native_result);
            }
        }

        internal napi_status napi_wrap(
            napi_env env,
            napi_value js_object,
            nint native_object,
            napi_finalize finalize_cb,
            nint finalize_hint,
            napi_ref* result)
            => CallInterop(
                ref _napi_wrap, env, js_object, native_object, finalize_cb, finalize_hint, result);

        internal napi_status napi_unwrap(napi_env env, napi_value js_object, out nint result)
            => CallInterop(ref _napi_unwrap, env, js_object, out result);

        internal napi_status napi_remove_wrap(napi_env env, napi_value js_object, out nint result)
            => CallInterop(ref _napi_remove_wrap, env, js_object, out result);

        internal napi_status napi_create_external(
            napi_env env,
            nint data,
            napi_finalize finalize_cb,
            nint finalize_hint,
            out napi_value result)
            => CallInterop(
                ref _napi_create_external, env, data, finalize_cb, finalize_hint, out result);

        internal napi_status napi_get_value_external(
            napi_env env, napi_value value, out nint result)
            => CallInterop(ref _napi_get_value_external, env, value, out result);

        internal napi_status napi_create_reference(
            napi_env env, napi_value value, uint initial_refcount, out napi_ref result)
            => CallInterop(ref _napi_create_reference, env, value, initial_refcount, out result);

        internal napi_status napi_delete_reference(napi_env env, napi_ref @ref)
            => CallInterop(ref _napi_delete_reference, env, @ref);

        internal napi_status napi_reference_ref(napi_env env, napi_ref @ref, nint result)
            => CallInterop(ref _napi_reference_ref, env, @ref, result);

        internal napi_status napi_reference_unref(napi_env env, napi_ref @ref, nint result)
            => CallInterop(ref _napi_reference_unref, env, @ref, result);

        internal napi_status napi_get_reference_value(
            napi_env env, napi_ref @ref, out napi_value result)
            => CallInterop(ref _napi_get_reference_value, env, @ref, out result);

        internal napi_status napi_open_handle_scope(napi_env env, out napi_handle_scope result)
            => CallInterop(ref _napi_open_handle_scope, env, out result);

        internal napi_status napi_close_handle_scope(napi_env env, napi_handle_scope scope)
            => CallInterop(ref _napi_close_handle_scope, env, scope);

        internal napi_status napi_open_escapable_handle_scope(
            napi_env env, out napi_escapable_handle_scope result)
            => CallInterop(ref _napi_open_escapable_handle_scope, env, out result);

        internal napi_status napi_close_escapable_handle_scope(
            napi_env env, napi_escapable_handle_scope scope)
            => CallInterop(ref _napi_close_escapable_handle_scope, env, scope);

        internal napi_status napi_escape_handle(napi_env env,
            napi_escapable_handle_scope scope, napi_value escapee, out napi_value result)
            => CallInterop(ref _napi_escape_handle, env, scope, escapee, out result);

        internal napi_status napi_throw(napi_env env, napi_value error)
            => CallInterop(ref _napi_throw, env, error);

        internal napi_status napi_throw_error(napi_env env, string? code, string msg)
            => CallInterop(ref _napi_throw_error, env, code, msg);

        internal napi_status napi_throw_type_error(napi_env env, string? code, string msg)
            => CallInterop(ref _napi_throw_type_error, env, code, msg);

        internal napi_status napi_throw_range_error(napi_env env, string? code, string msg)
            => CallInterop(ref _napi_throw_range_error, env, code, msg);

        internal napi_status node_api_throw_syntax_error(napi_env env, string? code, string msg)
            => CallInterop(ref _node_api_throw_syntax_error, env, code, msg);

        internal napi_status napi_is_error(napi_env env, napi_value value, out c_bool result)
            => CallInterop(ref _napi_is_error, env, value, out result);

        internal napi_status napi_is_exception_pending(napi_env env, out c_bool result)
            => CallInterop(ref _napi_is_exception_pending, env, out result);

        internal napi_status napi_get_and_clear_last_exception(napi_env env, out napi_value result)
            => CallInterop(ref _napi_get_and_clear_last_exception, env, out result);

        internal napi_status napi_is_arraybuffer(napi_env env, napi_value value, out c_bool result)
            => CallInterop(ref _napi_is_arraybuffer, env, value, out result);

        internal napi_status napi_create_arraybuffer(
            napi_env env, nuint byte_length, out nint data, out napi_value result)
            => CallInterop(ref _napi_create_arraybuffer, env, byte_length, out data, out result);

        internal napi_status napi_create_external_arraybuffer(
            napi_env env,
            nint external_data,
            nuint byte_length,
            napi_finalize finalize_cb,
            nint finalize_hint,
            out napi_value result)
            => CallInterop(
                ref _napi_create_external_arraybuffer,
                env,
                external_data,
                byte_length,
                finalize_cb,
                finalize_hint,
                out result);

        internal napi_status napi_get_arraybuffer_info(
            napi_env env, napi_value arraybuffer, out nint data, out nuint byte_length)
            => CallInterop(
                ref _napi_get_arraybuffer_info, env, arraybuffer, out data, out byte_length);

        internal napi_status napi_is_typedarray(napi_env env, napi_value value, out c_bool result)
            => CallInterop(ref _napi_is_typedarray, env, value, out result);

        internal napi_status napi_create_typedarray(
            napi_env env,
            napi_typedarray_type type,
            nuint length,
            napi_value arraybuffer,
            nuint byte_offset,
            out napi_value result)
            => CallInterop(
                ref _napi_create_typedarray,
                env,
                type,
                length,
                arraybuffer,
                byte_offset,
                out result);

        internal napi_status napi_get_typedarray_info(
            napi_env env,
            napi_value typedarray,
            out napi_typedarray_type type,
            out nuint length,
            out nint data,
            out napi_value arraybuffer,
            out nuint byte_offset)
            => CallInterop(
                ref _napi_get_typedarray_info,
                env,
                typedarray,
                out type,
                out length,
                out data,
                out arraybuffer,
                out byte_offset);

        internal napi_status napi_create_dataview(
            napi_env env,
            nuint length,
            napi_value arraybuffer,
            nuint byte_offset,
            out napi_value result)
            => CallInterop(
                ref _napi_create_dataview,
                env,
                length,
                arraybuffer,
                byte_offset,
                out result);

        internal napi_status napi_is_dataview(napi_env env, napi_value value, out c_bool result)
            => CallInterop(ref _napi_is_dataview, env, value, out result);

        internal napi_status napi_get_dataview_info(
            napi_env env,
            napi_value dataview,
            out nuint bytelength,
            out nint data,
            out napi_value arraybuffer,
            out nuint byte_offset)
            => CallInterop(
                ref _napi_get_dataview_info,
                env,
                dataview,
                out bytelength,
                out data,
                out arraybuffer,
                out byte_offset);


        internal napi_status napi_get_version(napi_env env, out uint result)
            => CallInterop(ref _napi_get_version, env, out result);

        internal napi_status napi_create_promise(
            napi_env env, out napi_deferred deferred, out napi_value promise)
            => CallInterop(ref _napi_create_promise, env, out deferred, out promise);

        internal napi_status napi_resolve_deferred(
            napi_env env, napi_deferred deferred, napi_value resolution)
            => CallInterop(ref _napi_resolve_deferred, env, deferred, resolution);

        internal napi_status napi_reject_deferred(
            napi_env env, napi_deferred deferred, napi_value rejection)
            => CallInterop(ref _napi_reject_deferred, env, deferred, rejection);

        internal napi_status napi_is_promise(napi_env env, napi_value value, out c_bool is_promise)
            => CallInterop(ref _napi_is_promise, env, value, out is_promise);

        internal napi_status napi_run_script(
            napi_env env, napi_value script, out napi_value result)
            => CallInterop(ref _napi_run_script, env, script, out result);

        internal napi_status napi_adjust_external_memory(
            napi_env env, long change_in_bytes, out long adjusted_value)
            => CallInterop(
                ref _napi_adjust_external_memory, env, change_in_bytes, out adjusted_value);

        internal napi_status napi_create_date(napi_env env, double time, out napi_value result)
            => CallInterop(ref _napi_create_date, env, time, out result);

        internal napi_status napi_is_date(napi_env env, napi_value value, out c_bool is_date)
            => CallInterop(ref _napi_is_date, env, value, out is_date);

        internal napi_status napi_get_date_value(napi_env env, napi_value value, out double result)
            => CallInterop(ref _napi_get_date_value, env, value, out result);

        internal napi_status napi_add_finalizer(
            napi_env env,
            napi_value js_object,
            nint native_object,
            napi_finalize finalize_cb,
            nint finalize_hint,
            napi_ref* result)
            => CallInterop(
                ref _napi_add_finalizer,
                env,
                js_object,
                native_object,
                finalize_cb,
                finalize_hint,
                result);

        internal napi_status napi_create_bigint_int64(
            napi_env env, long value, out napi_value result)
            => CallInterop(ref _napi_create_bigint_int64, env, value, out result);

        internal napi_status napi_create_bigint_uint64(
            napi_env env, ulong value, out napi_value result)
            => CallInterop(ref _napi_create_bigint_uint64, env, value, out result);


        internal napi_status napi_create_bigint_words(
            napi_env env, int sign_bit, nuint word_count, nint words, out napi_value result)
            => CallInterop(ref _napi_create_bigint_words, env, word_count, words, out result);


        internal napi_status napi_get_value_bigint_int64(
            napi_env env, napi_value value, out long result, out c_bool lossless)
            => CallInterop(ref _napi_get_value_bigint_int64, env, value, out result, out lossless);

        internal napi_status napi_get_value_bigint_uint64(
            napi_env env, napi_value value, out ulong result, out c_bool lossless)
            => CallInterop(
                ref _napi_get_value_bigint_uint64, env, value, out result, out lossless);

        internal napi_status napi_get_value_bigint_words(
            napi_env env, napi_value value, out int sign_bit, out nuint word_count, out uint words)
            => CallInterop(
                ref _napi_get_value_bigint_words,
                env,
                value,
                out sign_bit,
                out word_count,
                out words);

        internal napi_status napi_get_all_property_names(
            napi_env env,
            napi_value js_object,
            napi_key_collection_mode key_mode,
            napi_key_filter key_filter,
            napi_key_conversion key_conversion,
            out napi_value result)
            => CallInterop(
                ref _napi_get_all_property_names,
                env,
                js_object,
                key_mode,
                key_filter,
                key_conversion,
                out result);

        internal napi_status napi_set_instance_data(
            napi_env env, nint data, napi_finalize finalize_cb, nint finalize_hint)
            => CallInterop(ref _napi_set_instance_data, env, data, finalize_cb, finalize_hint);

        internal napi_status napi_get_instance_data(napi_env env, out nint data)
            => CallInterop(ref _napi_get_instance_data, env, out data);

        internal napi_status napi_detach_arraybuffer(napi_env env, napi_value arraybuffer)
            => CallInterop(ref _napi_detach_arraybuffer, env, arraybuffer);

        internal napi_status napi_is_detached_arraybuffer(
            napi_env env, napi_value value, out c_bool result)
            => CallInterop(ref _napi_detach_arraybuffer, env, value, out result);

        internal napi_status napi_type_tag_object(
            napi_env env, napi_value value, in napi_type_tag type_tag)
        {
            fixed (napi_type_tag* type_tag_native = &type_tag)
            {
                return CallInterop(ref _napi_type_tag_object, env, value, type_tag_native);
            }
        }

        internal napi_status napi_check_object_type_tag(
            napi_env env, napi_value value, in napi_type_tag type_tag, out c_bool result)
        {
            fixed (napi_type_tag* type_tag_native = &type_tag)
            fixed (c_bool* result_native = &result)
            {
                return CallInterop(
                    ref _napi_check_object_type_tag, env, value, type_tag_native, result_native);
            }
        }

        internal napi_status napi_object_freeze(napi_env env, napi_value js_object)
            => CallInterop(ref _napi_object_freeze, env, js_object);

        internal napi_status napi_object_seal(napi_env env, napi_value js_object)
            => CallInterop(ref _napi_object_seal, env, js_object);

        private nint GetExport(string name)
        {
            return NativeLibrary.GetExport(_libraryHandle, name);
        }

        private unsafe napi_status CallInterop<TResult>(
            ref nint field,
            napi_env env,
            out TResult result,
            [CallerMemberName] string functionName = "")
            where TResult : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, TResult*, napi_status>)funcHandle;
            fixed (TResult* result_native = &result)
            {
                return funcDelegate(env, result_native);
            }
        }

        private unsafe napi_status CallInterop<TResult1, TResult2>(
            ref nint field,
            napi_env env,
            out TResult1 result1,
            out TResult2 result2,
            [CallerMemberName] string functionName = "")
            where TResult1 : unmanaged
            where TResult2 : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, TResult1*, TResult2*, napi_status>)funcHandle;
            fixed (TResult1* result1_native = &result1)
            fixed (TResult2* result2_native = &result2)
            {
                return funcDelegate(env, result1_native, result2_native);
            }
        }

        private unsafe napi_status CallInterop<T, TResult>(
            ref nint field,
            napi_env env,
            T value,
            out TResult result,
            [CallerMemberName] string functionName = "")
            where T : unmanaged
            where TResult : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T, TResult*, napi_status>)funcHandle;
            fixed (TResult* result_native = &result)
            {
                return funcDelegate(env, value, result_native);
            }
        }

        private unsafe napi_status CallInterop<T, TResult1, TResult2>(
            ref nint field,
            napi_env env,
            T value,
            out TResult1 result1,
            out TResult2 result2,
            [CallerMemberName] string functionName = "")
            where T : unmanaged
            where TResult1 : unmanaged
            where TResult2 : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T, TResult1*, TResult2*, napi_status>)funcHandle;
            fixed (TResult1* result1_native = &result1)
            fixed (TResult2* result2_native = &result2)
            {
                return funcDelegate(env, value, result1_native, result2_native);
            }
        }

        private unsafe napi_status CallInterop<T, TResult1, TResult2, TResult3>(
            ref nint field,
            napi_env env,
            T value,
            out TResult1 result1,
            out TResult2 result2,
            out TResult3 result3,
            [CallerMemberName] string functionName = "")
            where T : unmanaged
            where TResult1 : unmanaged
            where TResult2 : unmanaged
            where TResult3 : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T, TResult1*, TResult2*, TResult3*, napi_status>)funcHandle;
            fixed (TResult1* result1_native = &result1)
            fixed (TResult2* result2_native = &result2)
            fixed (TResult3* result3_native = &result3)
            {
                return funcDelegate(env, value, result1_native, result2_native, result3_native);
            }
        }

        private unsafe napi_status CallInterop<T, TResult1, TResult2, TResult3, TResult4>(
            ref nint field,
            napi_env env,
            T value,
            out TResult1 result1,
            out TResult2 result2,
            out TResult3 result3,
            out TResult4 result4,
            [CallerMemberName] string functionName = "")
            where T : unmanaged
            where TResult1 : unmanaged
            where TResult2 : unmanaged
            where TResult3 : unmanaged
            where TResult4 : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T, TResult1*, TResult2*, TResult3*, TResult4*, napi_status>)funcHandle;
            fixed (TResult1* result1_native = &result1)
            fixed (TResult2* result2_native = &result2)
            fixed (TResult3* result3_native = &result3)
            fixed (TResult4* result4_native = &result4)
            {
                return funcDelegate(
                    env, value, result1_native, result2_native, result3_native, result4_native);
            }
        }

        private unsafe napi_status CallInterop<
            T, TResult1, TResult2, TResult3, TResult4, TResult5>(
            ref nint field,
            napi_env env,
            T value,
            out TResult1 result1,
            out TResult2 result2,
            out TResult3 result3,
            out TResult4 result4,
            out TResult5 result5,
            [CallerMemberName] string functionName = "")
            where T : unmanaged
            where TResult1 : unmanaged
            where TResult2 : unmanaged
            where TResult3 : unmanaged
            where TResult4 : unmanaged
            where TResult5 : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env,
                T,
                TResult1*,
                TResult2*,
                TResult3*,
                TResult4*,
                TResult5*,
                napi_status>)funcHandle;
            fixed (TResult1* result1_native = &result1)
            fixed (TResult2* result2_native = &result2)
            fixed (TResult3* result3_native = &result3)
            fixed (TResult4* result4_native = &result4)
            fixed (TResult5* result5_native = &result5)
            {
                return funcDelegate(
                    env,
                    value,
                    result1_native,
                    result2_native,
                    result3_native,
                    result4_native,
                    result5_native);
            }
        }

        private unsafe napi_status CallInterop<T1, T2, TResult>(
            ref nint field,
            napi_env env,
            T1 value1,
            T2 value2,
            out TResult result,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
            where TResult : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2, TResult*, napi_status>)funcHandle;
            fixed (TResult* result_native = &result)
            {
                return funcDelegate(env, value1, value2, result_native);
            }
        }

        private unsafe napi_status CallInterop<T1, T2>(
            ref nint field,
            napi_env env,
            T1 value1,
            T2* value2,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2*, napi_status>)funcHandle;
            return funcDelegate(env, value1, value2);
        }

        private unsafe napi_status CallInterop<T1, T2, T3>(
            ref nint field,
            napi_env env,
            T1 value1,
            T2* value2,
            T3* value3,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2*, T3*, napi_status>)funcHandle;
            return funcDelegate(env, value1, value2, value3);
        }

        private unsafe napi_status CallInterop<T1, T2, T3, TResult>(
            ref nint field,
            napi_env env,
            T1 value1,
            T2 value2,
            T3 value3,
            out TResult result,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where TResult : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2, T3, TResult*, napi_status>)funcHandle;
            fixed (TResult* result_native = &result)
            {
                return funcDelegate(env, value1, value2, value3, result_native);
            }
        }

        private unsafe napi_status CallInterop<T1, T2, T3, T4, TResult>(
            ref nint field,
            napi_env env,
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            out TResult result,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where TResult : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2, T3, T4, TResult*, napi_status>)funcHandle;
            fixed (TResult* result_native = &result)
            {
                return funcDelegate(env, value1, value2, value3, value4, result_native);
            }
        }

        private unsafe napi_status CallInterop<T1, T2, T3, T4, T5>(
            ref nint field,
            napi_env env,
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            T5* value5,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2, T3, T4, T5*, napi_status>)funcHandle;
            return funcDelegate(env, value1, value2, value3, value4, value5);
        }

        private unsafe napi_status CallInterop<T>(
            ref nint field,
            napi_env env,
            T value,
            [CallerMemberName] string functionName = "")
            where T : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T, napi_status>)funcHandle;
            return funcDelegate(env, value);
        }

        private unsafe napi_status CallInterop<T1, T2>(
            ref nint field,
            napi_env env,
            T1 value1,
            T2 value2,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2, napi_status>)funcHandle;
            return funcDelegate(env, value1, value2);
        }

        private unsafe napi_status CallInterop<T1, T2, T3>(
            ref nint field,
            napi_env env,
            T1 value1,
            T2 value2,
            T3 value3,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2, T3, napi_status>)funcHandle;
            return funcDelegate(env, value1, value2, value3);
        }

        private napi_status CallInterop(
            ref nint field,
            napi_env env,
            string? value1,
            string? value2,
            [CallerMemberName] string functionName = "")
        {
            nint funcHandle = (field != nint.Zero) ? field : field = GetExport(functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, byte*, byte*, napi_status>)funcHandle;

            Utf8StringMarshaller.ManagedToUnmanagedIn value1_marshaller = new();
            Utf8StringMarshaller.ManagedToUnmanagedIn value2_marshaller = new();
            try
            {
                int bufferSize = Utf8StringMarshaller.ManagedToUnmanagedIn.BufferSize;

                byte* value1_stackptr = stackalloc byte[bufferSize];
                value1_marshaller.FromManaged(value1, new Span<byte>(value1_stackptr, bufferSize));
                byte* value1_native = value1_marshaller.ToUnmanaged();

                byte* value2_stackptr = stackalloc byte[bufferSize];
                value2_marshaller.FromManaged(value2, new Span<byte>(value2_stackptr, bufferSize));
                byte* value2_native = value2_marshaller.ToUnmanaged();

                return funcDelegate(env, value1_native, value2_native);
            }
            finally
            {
                value1_marshaller.Free();
                value2_marshaller.Free();
            }
        }
    }
}
