package com.example.notediary;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class LoginActivity extends AppCompatActivity {

    private EditText editTextEmail, editTextPassword;
    private Button buttonLogin, buttonSubmitDocuments;
    private TextView textViewResponse;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        // Инициализация View
        textViewResponse = findViewById(R.id.textViewResponse);
        editTextEmail = findViewById(R.id.editTextEmail);
        editTextPassword = findViewById(R.id.editTextPassword);
        buttonLogin = findViewById(R.id.buttonLogin);
        Button buttonSubmitDocuments = findViewById(R.id.buttonSubmitDocuments);

        // Обработка нажатия на кнопку "Войти"
        buttonLogin.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                String email = editTextEmail.getText().toString().trim();
                String password = editTextPassword.getText().toString().trim();

                if (email.isEmpty() || password.isEmpty()) {
                    Toast.makeText(LoginActivity.this, "Заполните все поля!", Toast.LENGTH_SHORT).show();
                } else {
                    // Здесь можно добавить проверку логина и пароля
                    Toast.makeText(LoginActivity.this, "Вход выполнен!", Toast.LENGTH_SHORT).show();
                    // Переход на главную активность
                    Intent intent = new Intent(LoginActivity.this, MainActivity.class);
                    startActivity(intent);
                    finish();
                }
            }
        });
        // Кнопка для перехода на SubmitDocumentsActivity
        buttonSubmitDocuments.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Переход на следующую активность
                Intent intent = new Intent(LoginActivity.this, MotherActivity.class);
                startActivity(intent);
            }
        });
        // Кнопка для входа
        buttonLogin.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Создаем Retrofit
                ApiService apiService = ApiClient.getClient().create(ApiService.class);
                String email = editTextEmail.getText().toString();
                String Password = editTextPassword.getText().toString();
                UserLogin userLogin = new UserLogin(email, Password);
                // Переход на следующую активность
                // Вызов API
                Call<ApiResponse> call = apiService.sendData(userLogin);

// Обработка ответа
                call.enqueue(new Callback<ApiResponse>() {
                    @Override
                    public void onResponse(Call<ApiResponse> call, Response<ApiResponse> response) {
                        if (response.isSuccessful()) {
                            textViewResponse.setText("Данные успешно отправлены!");
                            Intent intent = new Intent(LoginActivity.this, StartActivity.class);
                            startActivity(intent);
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
        });


    }
}
