#include "pch-cpp.hpp"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <limits>



struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915;
struct WheelCollider_t4E35407C7AFEFA3DB30E9FFE3C38C9A5C5933481;



IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
struct U3CModuleU3E_t3E8E0BA57A4D4D0EF43301B668B802ED48E39035 
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F  : public RuntimeObject
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_pinvoke
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_com
{
};
struct IntPtr_t 
{
	void* ___m_value;
};
struct Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 
{
	float ___x;
	float ___y;
	float ___z;
	float ___w;
};
struct Single_t4530F2FF86FCB0DC29F35385CA1BD21BE294761C 
{
	float ___m_value;
};
struct Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 
{
	float ___x;
	float ___y;
	float ___z;
};
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915 
{
	union
	{
		struct
		{
		};
		uint8_t Void_t4861ACF8F4594C3437BB48B6E56783494B843915__padding[1];
	};
};
struct Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C  : public RuntimeObject
{
	intptr_t ___m_CachedPtr;
};
struct Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_marshaled_pinvoke
{
	intptr_t ___m_CachedPtr;
};
struct Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_marshaled_com
{
	intptr_t ___m_CachedPtr;
};
struct Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3  : public Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C
{
};
struct Collider_t1CC3163924FCD6C4CC2E816373A929C1E3D55E76  : public Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3
{
};
struct WheelCollider_t4E35407C7AFEFA3DB30E9FFE3C38C9A5C5933481  : public Collider_t1CC3163924FCD6C4CC2E816373A929C1E3D55E76
{
};
struct Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974_StaticFields
{
	Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 ___identityQuaternion;
};
struct Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_StaticFields
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___zeroVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___oneVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___upVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___downVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___leftVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___rightVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___forwardVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___backVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___positiveInfinityVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___negativeInfinityVector;
};
#ifdef __clang__
#pragma clang diagnostic pop
#endif



#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WheelCollider_set_motorTorque_m4958AAF7D867CF7570420F9BAFAF04DC904F02A8 (WheelCollider_t4E35407C7AFEFA3DB30E9FFE3C38C9A5C5933481* __this, float ___0_value, const RuntimeMethod* method) 
{
	typedef void (*WheelCollider_set_motorTorque_m4958AAF7D867CF7570420F9BAFAF04DC904F02A8_ftn) (WheelCollider_t4E35407C7AFEFA3DB30E9FFE3C38C9A5C5933481*, float);
	static WheelCollider_set_motorTorque_m4958AAF7D867CF7570420F9BAFAF04DC904F02A8_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (WheelCollider_set_motorTorque_m4958AAF7D867CF7570420F9BAFAF04DC904F02A8_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.WheelCollider::set_motorTorque(System.Single)");
	_il2cpp_icall_func(__this, ___0_value);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WheelCollider_set_steerAngle_m7BF83B27D8956355F873537939BE9F35CF3113C3 (WheelCollider_t4E35407C7AFEFA3DB30E9FFE3C38C9A5C5933481* __this, float ___0_value, const RuntimeMethod* method) 
{
	typedef void (*WheelCollider_set_steerAngle_m7BF83B27D8956355F873537939BE9F35CF3113C3_ftn) (WheelCollider_t4E35407C7AFEFA3DB30E9FFE3C38C9A5C5933481*, float);
	static WheelCollider_set_steerAngle_m7BF83B27D8956355F873537939BE9F35CF3113C3_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (WheelCollider_set_steerAngle_m7BF83B27D8956355F873537939BE9F35CF3113C3_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.WheelCollider::set_steerAngle(System.Single)");
	_il2cpp_icall_func(__this, ___0_value);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WheelCollider_GetWorldPose_m8C41FA2AE9ED543AB94A6E3226523A2FE83FA890 (WheelCollider_t4E35407C7AFEFA3DB30E9FFE3C38C9A5C5933481* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2* ___0_pos, Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974* ___1_quat, const RuntimeMethod* method) 
{
	typedef void (*WheelCollider_GetWorldPose_m8C41FA2AE9ED543AB94A6E3226523A2FE83FA890_ftn) (WheelCollider_t4E35407C7AFEFA3DB30E9FFE3C38C9A5C5933481*, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2*, Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974*);
	static WheelCollider_GetWorldPose_m8C41FA2AE9ED543AB94A6E3226523A2FE83FA890_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (WheelCollider_GetWorldPose_m8C41FA2AE9ED543AB94A6E3226523A2FE83FA890_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.WheelCollider::GetWorldPose(UnityEngine.Vector3&,UnityEngine.Quaternion&)");
	_il2cpp_icall_func(__this, ___0_pos, ___1_quat);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
