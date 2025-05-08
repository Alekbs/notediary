package com.example.notediary;

import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.Multipart;
import retrofit2.http.POST;
import retrofit2.http.Part;

public interface ApiService {
    @POST("api/users")
    Call<ApiResponse> sendData(@Body NotUser user);

    @POST("api/users/login")
    Call<ApiResponse> sendData(@Body UserLogin user);

    @Multipart
    @POST("api/users/upload")
    Call<ApiResponse> uploadImage(
            @Part MultipartBody.Part file,
            @Part("email") RequestBody email
    );
}
