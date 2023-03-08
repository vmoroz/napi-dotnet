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
        private nint _libraryHandle;

        [ThreadStatic] private static Interop? s_current;

        public static Interop? Current { get => s_current; set => s_current = value; }

        public enum MethodId
        {
            napi_get_last_error_info,
            napi_get_undefined,
            napi_get_null,
            napi_get_global,
            napi_get_boolean,
            napi_create_object,
            napi_create_array,
            napi_create_array_with_length,
            napi_create_double,
            napi_create_int32,
            napi_create_uint32,
            napi_create_int64,
            napi_create_string_latin1,
            napi_create_string_utf8,
            napi_create_string_utf16,
            napi_create_symbol,
            node_api_symbol_for,
            napi_create_function,
            napi_create_error,
            napi_create_type_error,
            napi_create_range_error,
            node_api_create_syntax_error,
            napi_typeof,
            napi_get_value_double,
            napi_get_value_int32,
            napi_get_value_uint32,
            napi_get_value_int64,
            napi_get_value_bool,
            napi_get_value_string_latin1,
            napi_get_value_string_utf8,
            napi_get_value_string_utf16,
            napi_coerce_to_bool,
            napi_coerce_to_number,
            napi_coerce_to_object,
            napi_coerce_to_string,
            napi_get_prototype,
            napi_get_property_names,
            napi_set_property,
            napi_has_property,
            napi_get_property,
            napi_delete_property,
            napi_has_own_property,
            napi_set_named_property,
            napi_has_named_property,
            napi_get_named_property,
            napi_set_element,
            napi_has_element,
            napi_get_element,
            napi_delete_element,
            napi_define_properties,
            napi_is_array,
            napi_get_array_length,
            napi_strict_equals,
            napi_call_function,
            napi_new_instance,
            napi_instanceof,
            napi_get_cb_info,
            napi_get_new_target,
            napi_define_class,
            napi_wrap,
            napi_unwrap,
            napi_remove_wrap,
            napi_create_external,
            napi_get_value_external,
            napi_create_reference,
            napi_delete_reference,
            napi_reference_ref,
            napi_reference_unref,
            napi_get_reference_value,
            napi_open_handle_scope,
            napi_close_handle_scope,
            napi_open_escapable_handle_scope,
            napi_close_escapable_handle_scope,
            napi_escape_handle,
            napi_throw,
            napi_throw_error,
            napi_throw_type_error,
            napi_throw_range_error,
            node_api_throw_syntax_error,
            napi_is_error,
            napi_is_exception_pending,
            napi_get_and_clear_last_exception,
            napi_is_arraybuffer,
            napi_create_arraybuffer,
            napi_create_external_arraybuffer,
            napi_get_arraybuffer_info,
            napi_is_typedarray,
            napi_create_typedarray,
            napi_get_typedarray_info,
            napi_create_dataview,
            napi_is_dataview,
            napi_get_dataview_info,
            napi_get_version,
            napi_create_promise,
            napi_resolve_deferred,
            napi_reject_deferred,
            napi_is_promise,
            napi_run_script,
            napi_adjust_external_memory,
            napi_create_date,
            napi_is_date,
            napi_get_date_value,
            napi_add_finalizer,
            napi_create_bigint_int64,
            napi_create_bigint_uint64,
            napi_create_bigint_words,
            napi_get_value_bigint_int64,
            napi_get_value_bigint_uint64,
            napi_get_value_bigint_words,
            napi_get_all_property_names,
            napi_set_instance_data,
            napi_get_instance_data,
            napi_detach_arraybuffer,
            napi_is_detached_arraybuffer,
            napi_type_tag_object,
            napi_check_object_type_tag,
            napi_object_freeze,
            napi_object_seal,
            // A special value to get method count. Must the last one.
            MethodCount,
        }

        private readonly nint[] _methods = new nint[(int)MethodId.MethodCount];

        public Interop(nint libraryHandle) => _libraryHandle = libraryHandle;

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

        internal static napi_status napi_get_last_error_info(napi_env env, out nint result)
            => CallInterop(s_current, MethodId.napi_get_last_error_info, env, out result);

        internal static napi_status napi_get_undefined(napi_env env, out napi_value result)
            => CallInterop(s_current, MethodId.napi_get_undefined, env, out result);

        internal static napi_status napi_get_null(napi_env env, out napi_value result)
            => CallInterop(s_current, MethodId.napi_get_null, env, out result);

        internal static napi_status napi_get_global(napi_env env, out napi_value result)
            => CallInterop(s_current, MethodId.napi_get_global, env, out result);

        internal static napi_status napi_get_boolean(
            napi_env env, c_bool value, out napi_value result)
            => CallInterop(s_current, MethodId.napi_get_boolean, env, value, out result);

        internal static napi_status napi_create_object(napi_env env, out napi_value result)
            => CallInterop(s_current, MethodId.napi_create_object, env, out result);

        internal static napi_status napi_create_array(napi_env env, out napi_value result)
            => CallInterop(s_current, MethodId.napi_create_array, env, out result);

        internal static napi_status napi_create_array_with_length(
            napi_env env, nuint length, out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_create_array_with_length, env, length, out result);

        internal static napi_status napi_create_double(
            napi_env env, double value, out napi_value result)
            => CallInterop(s_current, MethodId.napi_create_double, env, value, out result);

        internal static napi_status napi_create_int32(
            napi_env env, int value, out napi_value result)
            => CallInterop(s_current, MethodId.napi_create_int32, env, value, out result);

        internal static napi_status napi_create_uint32(
            napi_env env, uint value, out napi_value result)
            => CallInterop(s_current, MethodId.napi_create_uint32, env, value, out result);

        internal static napi_status napi_create_int64(
            napi_env env, long value, out napi_value result)
            => CallInterop(s_current, MethodId.napi_create_int64, env, value, out result);

        internal static napi_status napi_create_string_latin1(
            napi_env env, nint str, nuint length, out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_create_string_latin1, env, str, length, out result);

        internal static napi_status napi_create_string_utf8(
            napi_env env, nint str, nuint length, out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_create_string_utf8, env, str, length, out result);

        internal static napi_status napi_create_string_utf16(
            napi_env env, nint str, nuint length, out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_create_string_utf16, env, str, length, out result);

        internal static napi_status napi_create_symbol(
            napi_env env, napi_value description, out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_create_symbol, env, description, out result);

        internal static napi_status node_api_symbol_for(
            napi_env env, nint utf8name, nuint length, out napi_value result)
            => CallInterop(
                s_current, MethodId.node_api_symbol_for, env, utf8name, length, out result);

        internal static napi_status napi_create_function(
            napi_env env,
            nint utf8name,
            nuint length,
            napi_callback cb,
            nint data,
            out napi_value result)
            => CallInterop(
                s_current,
                MethodId.napi_create_function,
                env,
                utf8name,
                length,
                cb,
                data,
                out result);

        internal static napi_status napi_create_error(
            napi_env env, napi_value code, napi_value msg, out napi_value result)
            => CallInterop(s_current, MethodId.napi_create_error, env, code, msg, out result);

        internal static napi_status napi_create_type_error(
            napi_env env, napi_value code, napi_value msg, out napi_value result)
            => CallInterop(s_current, MethodId.napi_create_type_error, env, code, msg, out result);

        internal static napi_status napi_create_range_error(
            napi_env env, napi_value code, napi_value msg, out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_create_range_error, env, code, msg, out result);

        internal static napi_status node_api_create_syntax_error(
            napi_env env, napi_value code, napi_value msg, out napi_value result)
            => CallInterop(
                s_current, MethodId.node_api_create_syntax_error, env, code, msg, out result);

        internal static napi_status napi_typeof(
            napi_env env, napi_value value, out napi_valuetype result)
            => CallInterop(s_current, MethodId.napi_typeof, env, value, out result);

        internal static napi_status napi_get_value_double(
            napi_env env, napi_value value, out double result)
            => CallInterop(s_current, MethodId.napi_get_value_double, env, value, out result);

        internal static napi_status napi_get_value_int32(
            napi_env env, napi_value value, out int result)
            => CallInterop(s_current, MethodId.napi_get_value_int32, env, value, out result);

        internal static napi_status napi_get_value_uint32(
            napi_env env, napi_value value, out uint result)
            => CallInterop(s_current, MethodId.napi_get_value_uint32, env, value, out result);

        internal static napi_status napi_get_value_int64(
            napi_env env, napi_value value, out long result)
            => CallInterop(s_current, MethodId.napi_get_value_int64, env, value, out result);

        internal static napi_status napi_get_value_bool(
            napi_env env, napi_value value, out c_bool result)
            => CallInterop(s_current, MethodId.napi_get_value_bool, env, value, out result);

        internal static napi_status napi_get_value_string_latin1(
            napi_env env, napi_value value, nint buf, nuint bufsize, out nuint result)
            => CallInterop(
                s_current,
                MethodId.napi_get_value_string_latin1,
                env,
                value,
                buf,
                bufsize,
                out result);

        internal static napi_status napi_get_value_string_utf8(
            napi_env env, napi_value value, nint buf, nuint bufsize, out nuint result)
            => CallInterop(
                s_current,
                MethodId.napi_get_value_string_utf8,
                env,
                value,
                buf,
                bufsize,
                out result);

        internal static napi_status napi_get_value_string_utf16(
            napi_env env, napi_value value, nint buf, nuint bufsize, out nuint result)
            => CallInterop(
                s_current,
                MethodId.napi_get_value_string_utf16,
                env,
                value,
                buf,
                bufsize,
                out result);

        internal static napi_status napi_coerce_to_bool(
            napi_env env, napi_value value, out napi_value result)
            => CallInterop(s_current, MethodId.napi_coerce_to_bool, env, value, out result);

        internal static napi_status napi_coerce_to_number(
            napi_env env, napi_value value, out napi_value result)
            => CallInterop(s_current, MethodId.napi_coerce_to_number, env, value, out result);

        internal static napi_status napi_coerce_to_object(
            napi_env env, napi_value value, out napi_value result)
            => CallInterop(s_current, MethodId.napi_coerce_to_object, env, value, out result);

        internal static napi_status napi_coerce_to_string(
            napi_env env, napi_value value, out napi_value result)
            => CallInterop(s_current, MethodId.napi_coerce_to_string, env, value, out result);

        internal static napi_status napi_get_prototype(
            napi_env env, napi_value js_object, out napi_value result)
            => CallInterop(s_current, MethodId.napi_get_prototype, env, js_object, out result);

        internal static napi_status napi_get_property_names(
            napi_env env, napi_value js_object, out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_get_property_names, env, js_object, out result);

        internal static napi_status napi_set_property(
            napi_env env, napi_value js_object, napi_value key, napi_value value)
            => CallInterop(s_current, MethodId.napi_set_property, env, js_object, key, value);

        internal static napi_status napi_has_property(
            napi_env env, napi_value js_object, napi_value key, out c_bool result)
            => CallInterop(s_current, MethodId.napi_has_property, env, js_object, key, out result);

        internal static napi_status napi_get_property(
            napi_env env, napi_value js_object, napi_value key, out napi_value result)
            => CallInterop(s_current, MethodId.napi_get_property, env, js_object, key, out result);

        internal static napi_status napi_delete_property(
            napi_env env, napi_value js_object, napi_value key, out c_bool result)
            => CallInterop(
                s_current, MethodId.napi_delete_property, env, js_object, key, out result);

        internal static napi_status napi_has_own_property(
            napi_env env, napi_value js_object, napi_value key, out c_bool result)
            => CallInterop(
                s_current, MethodId.napi_has_own_property, env, js_object, key, out result);

        internal static napi_status napi_set_named_property(
            napi_env env, napi_value js_object, nint utf8name, napi_value value)
            => CallInterop(
                s_current, MethodId.napi_set_named_property, env, js_object, utf8name, value);

        internal static napi_status napi_has_named_property(
            napi_env env, napi_value js_object, nint utf8name, out c_bool result)
            => CallInterop(
                s_current, MethodId.napi_has_named_property, env, js_object, utf8name, out result);

        internal static napi_status napi_get_named_property(
            napi_env env, napi_value js_object, nint utf8name, out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_get_named_property, env, js_object, utf8name, out result);

        internal static napi_status napi_set_element(
            napi_env env, napi_value js_object, uint index, napi_value value)
            => CallInterop(s_current, MethodId.napi_set_element, env, js_object, index, value);

        internal static napi_status napi_has_element(
            napi_env env, napi_value js_object, uint index, out c_bool result)
            => CallInterop(
                s_current,
                MethodId.napi_has_element,
                env,
                js_object,
                index,
                out result);

        internal static napi_status napi_get_element(
            napi_env env, napi_value js_object, uint index, out napi_value result)
            => CallInterop(
                s_current,
                MethodId.napi_get_element,
                env,
                js_object,
                index,
                out result);

        internal static napi_status napi_delete_element(
            napi_env env, napi_value js_object, uint index, out c_bool result)
            => CallInterop(
                s_current, MethodId.napi_delete_element, env, js_object, index, out result);

        internal static napi_status napi_define_properties(
            napi_env env, napi_value js_object, nuint property_count, nint properties)
            => CallInterop(
                s_current,
                MethodId.napi_define_properties,
                env,
                js_object,
                property_count,
                properties);

        internal static napi_status napi_is_array(
            napi_env env, napi_value value, out c_bool result)
            => CallInterop(s_current, MethodId.napi_is_array, env, value, out result);

        internal static napi_status napi_get_array_length(
            napi_env env, napi_value value, out uint result)
            => CallInterop(s_current, MethodId.napi_get_array_length, env, value, out result);

        internal static napi_status napi_strict_equals(
            napi_env env, napi_value lhs, napi_value rhs, out c_bool result)
            => CallInterop(s_current, MethodId.napi_strict_equals, env, lhs, rhs, out result);

        internal static napi_status napi_call_function(napi_env env,
            napi_value recv, napi_value func, nuint argc, nint argv, out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_call_function, env, recv, func, argc, argv, out result);

        internal static napi_status napi_new_instance(napi_env env,
            napi_value constructor, nuint argc, nint argv, out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_new_instance, env, constructor, argc, argv, out result);

        internal static napi_status napi_instanceof(
            napi_env env, napi_value js_object, napi_value constructor, out c_bool result)
            => CallInterop(
                s_current, MethodId.napi_instanceof, env, js_object, constructor, out result);

        internal static napi_status napi_get_cb_info(
            napi_env env,              // [in] NAPI environment handle
            napi_callback_info cbinfo, // [in] Opaque callback-info handle
            nuint* argc,               // [in-out] Specifies the size of the provided argv array
                                       // and receives the actual count of args.
            napi_value* argv,          // [out] Array of values
            napi_value* this_arg,      // [out] Receives the JS 'this' arg for the call
            nint* data)                // [out] Receives the data pointer for the callback.
            => CallInterop(
                s_current, MethodId.napi_get_cb_info, env, cbinfo, argc, argv, this_arg, data);

        internal static napi_status napi_get_new_target(
            napi_env env, napi_callback_info cbinfo, out napi_value result)
            => CallInterop(s_current, MethodId.napi_get_new_target, env, cbinfo, out result);

        internal static napi_status napi_define_class(
            napi_env env,
            nint utf8name,
            nuint length,
            napi_callback constructor,
            nint data,
            nuint property_count,
            nint properties,
            out napi_value result)
            => CallInterop(
                s_current,
                MethodId.napi_define_class,
                env,
                utf8name,
                length,
                constructor,
                data,
                property_count,
                properties,
                out result);

        internal static napi_status napi_wrap(
            napi_env env,
            napi_value js_object,
            nint native_object,
            napi_finalize finalize_cb,
            nint finalize_hint,
            napi_ref* result)
            => CallInterop(
                s_current,
                MethodId.napi_wrap,
                env,
                js_object,
                native_object,
                finalize_cb,
                finalize_hint,
                result);

        internal static napi_status napi_unwrap(
            napi_env env, napi_value js_object, out nint result)
            => CallInterop(s_current, MethodId.napi_unwrap, env, js_object, out result);

        internal static napi_status napi_remove_wrap(
            napi_env env, napi_value js_object, out nint result)
            => CallInterop(s_current, MethodId.napi_remove_wrap, env, js_object, out result);

        internal static napi_status napi_create_external(
            napi_env env,
            nint data,
            napi_finalize finalize_cb,
            nint finalize_hint,
            out napi_value result)
            => CallInterop(
                s_current,
                MethodId.napi_create_external,
                env,
                data,
                finalize_cb,
                finalize_hint,
                out result);

        internal static napi_status napi_get_value_external(
            napi_env env, napi_value value, out nint result)
            => CallInterop(s_current, MethodId.napi_get_value_external, env, value, out result);

        internal static napi_status napi_create_reference(
            napi_env env, napi_value value, uint initial_refcount, out napi_ref result)
            => CallInterop(
                s_current,
                MethodId.napi_create_reference,
                env,
                value,
                initial_refcount,
                out result);

        internal static napi_status napi_delete_reference(napi_env env, napi_ref @ref)
            => CallInterop(s_current, MethodId.napi_delete_reference, env, @ref);

        internal static napi_status napi_reference_ref(napi_env env, napi_ref @ref, nint result)
            => CallInterop(s_current, MethodId.napi_reference_ref, env, @ref, result);

        internal static napi_status napi_reference_unref(napi_env env, napi_ref @ref, nint result)
            => CallInterop(s_current, MethodId.napi_reference_unref, env, @ref, result);

        internal static napi_status napi_get_reference_value(
            napi_env env, napi_ref @ref, out napi_value result)
            => CallInterop(s_current, MethodId.napi_get_reference_value, env, @ref, out result);

        static internal napi_status napi_open_handle_scope(
            napi_env env, out napi_handle_scope result)
            => CallInterop(s_current, MethodId.napi_open_handle_scope, env, out result);

        internal static napi_status napi_close_handle_scope(napi_env env, napi_handle_scope scope)
            => CallInterop(s_current, MethodId.napi_close_handle_scope, env, scope);

        internal static napi_status napi_open_escapable_handle_scope(
            napi_env env, out napi_escapable_handle_scope result)
            => CallInterop(s_current, MethodId.napi_open_escapable_handle_scope, env, out result);

        internal static napi_status napi_close_escapable_handle_scope(
            napi_env env, napi_escapable_handle_scope scope)
            => CallInterop(s_current, MethodId.napi_close_escapable_handle_scope, env, scope);

        internal static napi_status napi_escape_handle(napi_env env,
            napi_escapable_handle_scope scope, napi_value escapee, out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_escape_handle, env, scope, escapee, out result);

        internal static napi_status napi_throw(napi_env env, napi_value error)
            => CallInterop(s_current, MethodId.napi_throw, env, error);

        internal static napi_status napi_throw_error(napi_env env, string? code, string msg)
            => CallInterop(s_current, MethodId.napi_throw_error, env, code, msg);

        internal static napi_status napi_throw_type_error(napi_env env, string? code, string msg)
            => CallInterop(s_current, MethodId.napi_throw_type_error, env, code, msg);

        internal static napi_status napi_throw_range_error(napi_env env, string? code, string msg)
            => CallInterop(s_current, MethodId.napi_throw_range_error, env, code, msg);

        internal static napi_status node_api_throw_syntax_error(
            napi_env env, string? code, string msg)
            => CallInterop(s_current, MethodId.node_api_throw_syntax_error, env, code, msg);

        internal static napi_status napi_is_error(
            napi_env env, napi_value value, out c_bool result)
            => CallInterop(s_current, MethodId.napi_is_error, env, value, out result);

        internal static napi_status napi_is_exception_pending(napi_env env, out c_bool result)
            => CallInterop(s_current, MethodId.napi_is_exception_pending, env, out result);

        internal static napi_status napi_get_and_clear_last_exception(
            napi_env env, out napi_value result)
            => CallInterop(s_current, MethodId.napi_get_and_clear_last_exception, env, out result);

        internal static napi_status napi_is_arraybuffer(
            napi_env env, napi_value value, out c_bool result)
            => CallInterop(s_current, MethodId.napi_is_arraybuffer, env, value, out result);

        internal static napi_status napi_create_arraybuffer(
            napi_env env, nuint byte_length, out nint data, out napi_value result)
            => CallInterop(
                s_current,
                MethodId.napi_create_arraybuffer,
                env,
                byte_length,
                out data,
                out result);

        internal static napi_status napi_create_external_arraybuffer(
            napi_env env,
            nint external_data,
            nuint byte_length,
            napi_finalize finalize_cb,
            nint finalize_hint,
            out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_create_external_arraybuffer,
                env,
                external_data,
                byte_length,
                finalize_cb,
                finalize_hint,
                out result);

        internal static napi_status napi_get_arraybuffer_info(
            napi_env env, napi_value arraybuffer, out nint data, out nuint byte_length)
            => CallInterop(
                s_current,
                MethodId.napi_get_arraybuffer_info,
                env,
                arraybuffer,
                out data,
                out byte_length);

        internal static napi_status napi_is_typedarray(
            napi_env env, napi_value value, out c_bool result)
            => CallInterop(s_current, MethodId.napi_is_typedarray, env, value, out result);

        internal static napi_status napi_create_typedarray(
            napi_env env,
            napi_typedarray_type type,
            nuint length,
            napi_value arraybuffer,
            nuint byte_offset,
            out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_create_typedarray,
                env,
                type,
                length,
                arraybuffer,
                byte_offset,
                out result);

        internal static napi_status napi_get_typedarray_info(
            napi_env env,
            napi_value typedarray,
            out napi_typedarray_type type,
            out nuint length,
            out nint data,
            out napi_value arraybuffer,
            out nuint byte_offset)
            => CallInterop(
                s_current, MethodId.napi_get_typedarray_info,
                env,
                typedarray,
                out type,
                out length,
                out data,
                out arraybuffer,
                out byte_offset);

        internal static napi_status napi_create_dataview(
            napi_env env,
            nuint length,
            napi_value arraybuffer,
            nuint byte_offset,
            out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_create_dataview,
                env,
                length,
                arraybuffer,
                byte_offset,
                out result);

        internal static napi_status napi_is_dataview(
            napi_env env, napi_value value, out c_bool result)
            => CallInterop(s_current, MethodId.napi_is_dataview, env, value, out result);

        internal static napi_status napi_get_dataview_info(
            napi_env env,
            napi_value dataview,
            out nuint bytelength,
            out nint data,
            out napi_value arraybuffer,
            out nuint byte_offset)
            => CallInterop(
                s_current, MethodId.napi_get_dataview_info,
                env,
                dataview,
                out bytelength,
                out data,
                out arraybuffer,
                out byte_offset);


        internal static napi_status napi_get_version(napi_env env, out uint result)
            => CallInterop(s_current, MethodId.napi_get_version, env, out result);

        internal static napi_status napi_create_promise(
            napi_env env, out napi_deferred deferred, out napi_value promise)
            => CallInterop(
                s_current, MethodId.napi_create_promise, env, out deferred, out promise);

        internal static napi_status napi_resolve_deferred(
            napi_env env, napi_deferred deferred, napi_value resolution)
            => CallInterop(s_current, MethodId.napi_resolve_deferred, env, deferred, resolution);

        internal static napi_status napi_reject_deferred(
            napi_env env, napi_deferred deferred, napi_value rejection)
            => CallInterop(s_current, MethodId.napi_reject_deferred, env, deferred, rejection);

        internal static napi_status napi_is_promise(
            napi_env env, napi_value value, out c_bool is_promise)
            => CallInterop(s_current, MethodId.napi_is_promise, env, value, out is_promise);

        internal static napi_status napi_run_script(
            napi_env env, napi_value script, out napi_value result)
            => CallInterop(s_current, MethodId.napi_run_script, env, script, out result);

        internal static napi_status napi_adjust_external_memory(
            napi_env env, long change_in_bytes, out long adjusted_value)
            => CallInterop(
                s_current,
                MethodId.napi_adjust_external_memory,
                env,
                change_in_bytes,
                out adjusted_value);

        internal static napi_status napi_create_date(
            napi_env env, double time, out napi_value result)
            => CallInterop(s_current, MethodId.napi_create_date, env, time, out result);

        internal static napi_status napi_is_date(
            napi_env env, napi_value value, out c_bool is_date)
            => CallInterop(s_current, MethodId.napi_is_date, env, value, out is_date);

        internal static napi_status napi_get_date_value(
            napi_env env, napi_value value, out double result)
            => CallInterop(s_current, MethodId.napi_get_date_value, env, value, out result);

        internal static napi_status napi_add_finalizer(
            napi_env env,
            napi_value js_object,
            nint native_object,
            napi_finalize finalize_cb,
            nint finalize_hint,
            napi_ref* result)
            => CallInterop(
                s_current, MethodId.napi_add_finalizer,
                env,
                js_object,
                native_object,
                finalize_cb,
                finalize_hint,
                result);

        internal static napi_status napi_create_bigint_int64(
            napi_env env, long value, out napi_value result)
            => CallInterop(s_current, MethodId.napi_create_bigint_int64, env, value, out result);

        internal static napi_status napi_create_bigint_uint64(
            napi_env env, ulong value, out napi_value result)
            => CallInterop(s_current, MethodId.napi_create_bigint_uint64, env, value, out result);


        internal static napi_status napi_create_bigint_words(
            napi_env env, int sign_bit, nuint word_count, nint words, out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_create_bigint_words, env, word_count, words, out result);


        internal static napi_status napi_get_value_bigint_int64(
            napi_env env, napi_value value, out long result, out c_bool lossless)
            => CallInterop(
                s_current,
                MethodId.napi_get_value_bigint_int64,
                env,
                value,
                out result,
                out lossless);

        internal static napi_status napi_get_value_bigint_uint64(
            napi_env env, napi_value value, out ulong result, out c_bool lossless)
            => CallInterop(
                s_current,
                MethodId.napi_get_value_bigint_uint64,
                env,
                value,
                out result,
                out lossless);

        internal static napi_status napi_get_value_bigint_words(
            napi_env env, napi_value value, out int sign_bit, out nuint word_count, ulong* words)
            => CallInterop(
                s_current, MethodId.napi_get_value_bigint_words,
                env,
                value,
                out sign_bit,
                out word_count,
                words);

        internal static napi_status napi_get_all_property_names(
            napi_env env,
            napi_value js_object,
            napi_key_collection_mode key_mode,
            napi_key_filter key_filter,
            napi_key_conversion key_conversion,
            out napi_value result)
            => CallInterop(
                s_current, MethodId.napi_get_all_property_names,
                env,
                js_object,
                key_mode,
                key_filter,
                key_conversion,
                out result);

        internal static napi_status napi_set_instance_data(
            napi_env env, nint data, napi_finalize finalize_cb, nint finalize_hint)
            => CallInterop(
                s_current, MethodId.napi_set_instance_data, env, data, finalize_cb, finalize_hint);

        internal static napi_status napi_get_instance_data(napi_env env, out nint data)
            => CallInterop(s_current, MethodId.napi_get_instance_data, env, out data);

        internal static napi_status napi_detach_arraybuffer(napi_env env, napi_value arraybuffer)
            => CallInterop(s_current, MethodId.napi_detach_arraybuffer, env, arraybuffer);

        internal static napi_status napi_is_detached_arraybuffer(
            napi_env env, napi_value value, out c_bool result)
            => CallInterop(
                s_current, MethodId.napi_is_detached_arraybuffer, env, value, out result);

        internal static napi_status napi_type_tag_object(
            napi_env env, napi_value value, in napi_type_tag type_tag)
        {
            fixed (napi_type_tag* type_tag_native = &type_tag)
            {
                return CallInterop(
                    s_current, MethodId.napi_type_tag_object, env, value, type_tag_native);
            }
        }

        internal static napi_status napi_check_object_type_tag(
            napi_env env, napi_value value, in napi_type_tag type_tag, out c_bool result)
        {
            fixed (napi_type_tag* type_tag_native = &type_tag)
            fixed (c_bool* result_native = &result)
            {
                return CallInterop(
                    s_current,
                    MethodId.napi_check_object_type_tag,
                    env,
                    value,
                    type_tag_native,
                    result_native);
            }
        }

        internal static napi_status napi_object_freeze(napi_env env, napi_value js_object)
            => CallInterop(s_current, MethodId.napi_object_freeze, env, js_object);

        internal static napi_status napi_object_seal(napi_env env, napi_value js_object)
            => CallInterop(s_current, MethodId.napi_object_seal, env, js_object);

        private nint GetExport(MethodId methodId, string methodName)
        {
            nint methodPtr = _methods[(int)methodId];
            if (methodPtr == nint.Zero)
            {
                methodPtr = NativeLibrary.GetExport(_libraryHandle, methodName);
                _methods[(int)methodId] = methodPtr;
            }

            return methodPtr;
        }

        private static unsafe napi_status CallInterop<TResult>(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            out TResult result,
            [CallerMemberName] string functionName = "")
            where TResult : unmanaged
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, TResult*, napi_status>)funcHandle;
            fixed (TResult* result_native = &result)
            {
                return funcDelegate(env, result_native);
            }
        }

        private static unsafe napi_status CallInterop<TResult1, TResult2>(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            out TResult1 result1,
            out TResult2 result2,
            [CallerMemberName] string functionName = "")
            where TResult1 : unmanaged
            where TResult2 : unmanaged
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, TResult1*, TResult2*, napi_status>)funcHandle;
            fixed (TResult1* result1_native = &result1)
            fixed (TResult2* result2_native = &result2)
            {
                return funcDelegate(env, result1_native, result2_native);
            }
        }

        private static unsafe napi_status CallInterop<T, TResult>(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            T value,
            out TResult result,
            [CallerMemberName] string functionName = "")
            where T : unmanaged
            where TResult : unmanaged
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T, TResult*, napi_status>)funcHandle;
            fixed (TResult* result_native = &result)
            {
                return funcDelegate(env, value, result_native);
            }
        }

        private static unsafe napi_status CallInterop<T, TResult1, TResult2>(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            T value,
            out TResult1 result1,
            out TResult2 result2,
            [CallerMemberName] string functionName = "")
            where T : unmanaged
            where TResult1 : unmanaged
            where TResult2 : unmanaged
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T, TResult1*, TResult2*, napi_status>)funcHandle;
            fixed (TResult1* result1_native = &result1)
            fixed (TResult2* result2_native = &result2)
            {
                return funcDelegate(env, value, result1_native, result2_native);
            }
        }

        private static unsafe napi_status CallInterop<T, TResult1, TResult2, TResult3>(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            T value,
            out TResult1 result1,
            out TResult2 result2,
            TResult3* result3,
            [CallerMemberName] string functionName = "")
            where T : unmanaged
            where TResult1 : unmanaged
            where TResult2 : unmanaged
            where TResult3 : unmanaged
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T, TResult1*, TResult2*, TResult3*, napi_status>)funcHandle;
            fixed (TResult1* result1_native = &result1)
            fixed (TResult2* result2_native = &result2)
            {
                return funcDelegate(env, value, result1_native, result2_native, result3);
            }
        }

        private static unsafe napi_status CallInterop<T, TResult1, TResult2, TResult3, TResult4>(
            Interop? interop,
            MethodId methodId,
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
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
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

        private static unsafe napi_status CallInterop<
            T, TResult1, TResult2, TResult3, TResult4, TResult5>(
            Interop? interop,
            MethodId methodId,
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
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
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

        private static unsafe napi_status CallInterop<T1, T2, TResult>(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            T1 value1,
            T2 value2,
            out TResult result,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
            where TResult : unmanaged
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2, TResult*, napi_status>)funcHandle;
            fixed (TResult* result_native = &result)
            {
                return funcDelegate(env, value1, value2, result_native);
            }
        }

        private static unsafe napi_status CallInterop<T1, T2>(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            T1 value1,
            T2* value2,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2*, napi_status>)funcHandle;
            return funcDelegate(env, value1, value2);
        }

        private static unsafe napi_status CallInterop<T1, T2, T3>(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            T1 value1,
            T2* value2,
            T3* value3,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2*, T3*, napi_status>)funcHandle;
            return funcDelegate(env, value1, value2, value3);
        }

        private static unsafe napi_status CallInterop<T1, T2, T3, TResult>(
            Interop? interop,
            MethodId methodId,
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
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2, T3, TResult*, napi_status>)funcHandle;
            fixed (TResult* result_native = &result)
            {
                return funcDelegate(env, value1, value2, value3, result_native);
            }
        }

        private static unsafe napi_status CallInterop<T1, T2, T3, T4, TResult>(
            Interop? interop,
            MethodId methodId,
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
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2, T3, T4, TResult*, napi_status>)funcHandle;
            fixed (TResult* result_native = &result)
            {
                return funcDelegate(env, value1, value2, value3, value4, result_native);
            }
        }

        private static unsafe napi_status CallInterop<T1, T2, T3, T4, T5>(
            Interop? interop,
            MethodId methodId,
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
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2, T3, T4, T5*, napi_status>)funcHandle;
            return funcDelegate(env, value1, value2, value3, value4, value5);
        }

        private static unsafe napi_status CallInterop<T1, T2, T3, T4, T5, T6, TResult>(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            T5 value5,
            T6 value6,
            out TResult result,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where TResult : unmanaged
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2, T3, T4, T5, T6, TResult*, napi_status>)funcHandle;
            fixed (TResult* result_native = &result)
            {
                return funcDelegate(
                    env, value1, value2, value3, value4, value5, value6, result_native);
            }
        }

        private static unsafe napi_status CallInterop<T>(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            T value,
            [CallerMemberName] string functionName = "")
            where T : unmanaged
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T, napi_status>)funcHandle;
            return funcDelegate(env, value);
        }

        private static unsafe napi_status CallInterop<T1, T2>(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            T1 value1,
            T2 value2,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2, napi_status>)funcHandle;
            return funcDelegate(env, value1, value2);
        }

        private static unsafe napi_status CallInterop<T1, T2, T3>(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            T1 value1,
            T2 value2,
            T3 value3,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2, T3, napi_status>)funcHandle;
            return funcDelegate(env, value1, value2, value3);
        }

        private static unsafe napi_status CallInterop<T1, T2, T3, T4, T5>(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            T1 value1,
            T2* value2,
            T3* value3,
            T4* value4,
            T5* value5,
            [CallerMemberName] string functionName = "")
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
            var funcDelegate = (delegate* unmanaged[Cdecl]<
                napi_env, T1, T2*, T3*, T4*, T5*, napi_status>)funcHandle;
            return funcDelegate(env, value1, value2, value3, value4, value5);
        }

        private static napi_status CallInterop(
            Interop? interop,
            MethodId methodId,
            napi_env env,
            string? value1,
            string? value2,
            [CallerMemberName] string functionName = "")
        {
            ArgumentNullException.ThrowIfNull(interop);
            nint funcHandle = interop.GetExport(methodId, functionName);
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
