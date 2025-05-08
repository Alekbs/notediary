package com.example.notediary;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import androidx.appcompat.app.AppCompatActivity;

public class MotherActivity extends AppCompatActivity {

    private EditText editTextMotherName, editTextMotherPassport, editTextMotherAddress, editTextMotherWorkplace, editTextMotherContact;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_mother);

        // Инициализация полей
        editTextMotherName = findViewById(R.id.editTextMotherName);
        editTextMotherPassport = findViewById(R.id.editTextMotherPassport);
        editTextMotherAddress = findViewById(R.id.editTextMotherAddress);
        editTextMotherWorkplace = findViewById(R.id.editTextMotherWorkplace);
        editTextMotherContact = findViewById(R.id.editTextMotherContact);

        Button buttonNextToFather = findViewById(R.id.buttonNextToFather);




        // Переход к следующей активности
        buttonNextToFather.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(MotherActivity.this, FatherActivity.class);

                // Передача данных матери в следующую активность
                SharedPreferences sharedPreferences = getSharedPreferences("UserData", MODE_PRIVATE);
                SharedPreferences.Editor editor = sharedPreferences.edit();



// Сохранение значений в SharedPreferences
                editor.putString("motherName", editTextMotherName.getText().toString());
                editor.putString("motherPassport", editTextMotherPassport.getText().toString());
                editor.putString("motherAddress", editTextMotherAddress.getText().toString());
                editor.putString("motherWorkplace", editTextMotherWorkplace.getText().toString());
                editor.putString("motherContact", editTextMotherContact.getText().toString());
                editor.apply();
                startActivity(intent);
            }
        });
    }
}
