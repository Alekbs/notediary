package com.example.notediary;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;

public class EmailRegistrationActivity extends AppCompatActivity {

    private EditText editTextEmail;
    private Button buttonRegister;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_email_registration);

        editTextEmail = findViewById(R.id.editTextEmail);

        buttonRegister = findViewById(R.id.buttonNextToConstent);

        // Обработчик нажатия на кнопку регистрации
        buttonRegister.setOnClickListener(v -> {
            String email = editTextEmail.getText().toString().trim();

            if (email.isEmpty()) {
                Toast.makeText(EmailRegistrationActivity.this, "Пожалуйста, введите email", Toast.LENGTH_SHORT).show();
            } else {
                // Отправка email в следующую активность
                Intent intent = new Intent(EmailRegistrationActivity.this, ConsentActivity.class);
                SharedPreferences sharedPreferences = getSharedPreferences("UserData", MODE_PRIVATE);
                SharedPreferences.Editor editor = sharedPreferences.edit();
                editor.putString("email", email);
                editor.apply();
                startActivity(intent);

                // Можно добавить логику для отправки email на сервер или выполнение других действий
                //Toast.makeText(EmailRegistrationActivity.this, "Регистрация прошла успешно!", Toast.LENGTH_SHORT).show();
            }
        });
    }
}
