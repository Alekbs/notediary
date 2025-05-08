package com.example.notediary;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import androidx.appcompat.app.AppCompatActivity;

public class FatherActivity extends AppCompatActivity {

    private EditText editTextFatherName, editTextFatherPassport, editTextFatherAddress, editTextFatherWorkplace, editTextFatherContact;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_father);

        // Инициализация полей
        editTextFatherName = findViewById(R.id.editTextFatherName);
        editTextFatherPassport = findViewById(R.id.editTextFatherPassport);
        editTextFatherAddress = findViewById(R.id.editTextFatherAddress);
        editTextFatherWorkplace = findViewById(R.id.editTextFatherWorkplace);
        editTextFatherContact = findViewById(R.id.editTextFatherContact);

        Button buttonNextToChild = findViewById(R.id.buttonNextToChild);

        // Переход к следующей активности
        buttonNextToChild.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(FatherActivity.this, ChildActivity.class);

                // Передача данных матери в следующую активность
                SharedPreferences sharedPreferences = getSharedPreferences("UserData", MODE_PRIVATE);
                SharedPreferences.Editor editor = sharedPreferences.edit();



// Сохранение значений в SharedPreferences
                editor.putString("fatherName", editTextFatherName.getText().toString());
                editor.putString("fatherPassport", editTextFatherPassport.getText().toString());
                editor.putString("fatherAddress", editTextFatherAddress.getText().toString());
                editor.putString("fatherWorkplace", editTextFatherWorkplace.getText().toString());
                editor.putString("fatherContact", editTextFatherContact.getText().toString());
                editor.apply();
                startActivity(intent);
            }
        });
    }
}
