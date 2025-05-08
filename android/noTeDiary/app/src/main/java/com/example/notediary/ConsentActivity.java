package com.example.notediary;

import android.content.SharedPreferences;
import android.database.Cursor;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Bundle;
import android.provider.MediaStore;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import androidx.loader.content.CursorLoader;

import javax.mail.Message;
import javax.mail.MessagingException;
import javax.mail.Session;
import javax.mail.Transport;
import javax.mail.internet.InternetAddress;
import javax.mail.internet.MimeMessage;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;
import android.content.Intent;

import com.google.gson.Gson;

import okhttp3.MediaType;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;


public class ConsentActivity extends AppCompatActivity {

    private CheckBox checkBoxDataProcessing, checkBoxPsychologicalSupport;
    private Button buttonGoToLogin;
    private Button buttonSendRequest;
    private EditText editTextEmail;
    private String email, childName, childAddress, learningWorkplace, childContact, avatarUriString;
    private String motherName, motherPassport, motherAddress, motherWorkplace, motherContact;
    private String fatherName, fatherPassport, fatherAddress, fatherWorkplace, fatherContact;
    private Uri avatarUri;
    private TextView textViewResponse;
    private ImageView avatarImageView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_consents);
        buttonGoToLogin =  findViewById(R.id.buttonGoToLogin);
        textViewResponse = findViewById(R.id.textViewResponse);
        editTextEmail = findViewById((R.id.editTextEmail));
        checkBoxDataProcessing = findViewById(R.id.checkBoxDataProcessing);
        checkBoxPsychologicalSupport = findViewById(R.id.checkBoxPsychologicalSupport);
        buttonSendRequest = findViewById(R.id.buttonConfirmConsent);

        SharedPreferences sharedPreferences = getSharedPreferences("UserData", MODE_PRIVATE);

        // Получение данных из хранилища
        childName = sharedPreferences.getString("ChildName", null);
        childAddress = sharedPreferences.getString("ChildAddress", null);
        learningWorkplace = sharedPreferences.getString("LearningWorkplace", null);
        childContact = sharedPreferences.getString("ChildContact", null);

        motherName = sharedPreferences.getString("motherName", null);
        motherPassport = sharedPreferences.getString("motherPassport", null);
        motherAddress = sharedPreferences.getString("motherAddress", null);
        motherWorkplace = sharedPreferences.getString("motherWorkplace", null);
        motherContact = sharedPreferences.getString("motherContact", null);

        fatherName = sharedPreferences.getString("fatherName", null);
        fatherPassport = sharedPreferences.getString("fatherPassport", null);
        fatherAddress = sharedPreferences.getString("fatherAddress", null);
        fatherWorkplace = sharedPreferences.getString("fatherWorkplace", null);
        fatherContact = sharedPreferences.getString("fatherContact", null);
        avatarUriString = sharedPreferences.getString("AvatarUri", null);
        avatarUri = Uri.parse(avatarUriString);

        avatarImageView = findViewById(R.id.avatarImageView);

        if (avatarUri != null) {

            // Если изображение находится на устройстве, можно напрямую установить URI
            avatarImageView.setImageURI(avatarUri);
        } else {
            Toast.makeText(this, "Аватар не найден", Toast.LENGTH_SHORT).show();
        }
        // Обработчик нажатия на кнопку подтверждения согласия
        // Кнопка для отправки данных на сервер
        buttonSendRequest.setOnClickListener(v -> sendDataToServer());
        buttonGoToLogin.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Переход на следующую активность
                Intent intent = new Intent(ConsentActivity.this, LoginActivity.class);
                startActivity(intent);
            }
        });
    }

    // Метод для отправки данных на сервер
    private void sendDataToServer() {
        boolean isDataProcessingAgreed = checkBoxDataProcessing.isChecked();
        boolean isPsychologicalSupportAgreed = checkBoxPsychologicalSupport.isChecked();
        String email = editTextEmail.getText().toString().trim();
        if (email.isEmpty()) {
            Toast.makeText(ConsentActivity.this, "Пожалуйста, введите email", Toast.LENGTH_SHORT).show();
        }
        else if(!isDataProcessingAgreed || !isPsychologicalSupportAgreed) {
            Toast.makeText(ConsentActivity.this, "Пожалуйста, согласитесь с условиями", Toast.LENGTH_SHORT).show();
        } else {
            // Если оба согласия выбраны
            Toast.makeText(ConsentActivity.this, "Вы выбрали оба согласия", Toast.LENGTH_SHORT).show();



            ApiService apiService = ApiClient.getClient().create(ApiService.class);



            // Формирование объекта User
            NotUser user = new NotUser(childName, childAddress, learningWorkplace, childContact,
                    motherName, motherPassport, motherAddress, motherWorkplace, motherContact,
                    fatherName, fatherPassport, fatherAddress, fatherWorkplace, fatherContact, email);

            // Сериализация объекта в JSON
            Gson gson = new Gson();
            String json = gson.toJson(user);

            // Выводим JSON в консоль
            System.out.println("JSON перед отправкой:");
            System.out.println(json);

            // Вызов API
            Call<ApiResponse> call = apiService.sendData(user);

            // Обработка ответа
            call.enqueue(new Callback<ApiResponse>() {
                @Override
                public void onResponse(Call<ApiResponse> call, Response<ApiResponse> response) {
                    if (response.isSuccessful()) {
                        textViewResponse.setText("Данные успешно отправлены!");
                        uploadAvatar(avatarUri, email);

                    } else {
                        textViewResponse.setText("Ошибка: " + response.code() + " - " + response.message());
                    }
                }

                @Override
                public void onFailure(Call<ApiResponse> call, Throwable t) {
                    textViewResponse.setText("Ошибка: " + t.getMessage());
                }
            });
        }

    }
    private void uploadAvatar(Uri avatarUri, String email) {
        File file = createFileFromUri(avatarUri);

        RequestBody requestFile = RequestBody.create(MediaType.parse("image/*"), file);
        MultipartBody.Part body = MultipartBody.Part.createFormData("file", file.getName(), requestFile);
        RequestBody emailPart = RequestBody.create(MediaType.parse("text/plain"), email);


        ApiService apiService = ApiClient.getClient().create(ApiService.class);

        Call<ApiResponse> call = apiService.uploadImage(body, emailPart);
        call.enqueue(new Callback<ApiResponse>() {
            @Override
            public void onResponse(Call<ApiResponse> call, Response<ApiResponse> response) {
                if (response.isSuccessful()) {
                    Log.d("Upload", "Изображение успешно загружено!");
                } else {
                    Log.e("Upload", "Ошибка загрузки: " + response.message());
                }
            }

            @Override
            public void onFailure(Call<ApiResponse> call, Throwable t) {
                Log.e("Upload", "Ошибка сети: " + t.getMessage());
            }
        });
    }

    // Метод для получения реального пути файла из URI
    private String getRealPathFromURI(Uri contentUri) {
        String[] proj = { MediaStore.Images.Media.DATA };
        CursorLoader loader = new CursorLoader(this, contentUri, proj, null, null, null);
        Cursor cursor = loader.loadInBackground();
        int column_index = cursor.getColumnIndexOrThrow(MediaStore.Images.Media.DATA);
        cursor.moveToFirst();
        String path = cursor.getString(column_index);
        cursor.close();
        return path;
    }
    private File createFileFromUri(Uri uri) {
        InputStream inputStream = null;
        File tempFile = null;
        try {
            // Открываем InputStream для URI
            inputStream = getContentResolver().openInputStream(uri);
            if (inputStream == null) {
                Log.e("Upload", "Failed to open InputStream for URI: " + uri.toString());
                return null;
            }

            // Создаем временный файл
            tempFile = File.createTempFile("avatar_", ".jpg", getCacheDir());
            tempFile.deleteOnExit(); // Удалить файл при завершении работы приложения

            // Записываем данные из InputStream в файл
            FileOutputStream outputStream = new FileOutputStream(tempFile);
            byte[] buffer = new byte[1024];
            int length;
            while ((length = inputStream.read(buffer)) > 0) {
                outputStream.write(buffer, 0, length);
            }
            outputStream.flush();
            outputStream.close();

            return tempFile;

        } catch (FileNotFoundException e) {
            Log.e("Upload", "File not found: " + e.getMessage());
        } catch (IOException e) {
            Log.e("Upload", "Error writing to temp file: " + e.getMessage());
        } finally {
            if (inputStream != null) {
                try {
                    inputStream.close();
                } catch (IOException e) {
                    Log.e("Upload", "Error closing input stream: " + e.getMessage());
                }
            }
        }
        return null;
    }


}
