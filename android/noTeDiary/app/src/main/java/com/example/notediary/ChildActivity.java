package com.example.notediary;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import androidx.appcompat.app.AppCompatActivity;

public class ChildActivity extends AppCompatActivity {

    private EditText editTextChildName, editTextChildAddress, editTextLearningWorkplace, editTextChildContact;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_child);

        // Инициализация полей
        editTextChildName = findViewById(R.id.editTextChildName);
        editTextChildAddress = findViewById(R.id.editTextChildAddress);
        editTextLearningWorkplace = findViewById(R.id.editTextLearningWorkplace);
        editTextChildContact = findViewById(R.id.editTextChildContact);

        Button buttonNextToChild = findViewById(R.id.buttonNextToChild);

        // Переход к следующей активности
        buttonNextToChild.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(ChildActivity.this, AvatarActivity.class);

                // Сохранение значений в SharedPreferences
                SharedPreferences sharedPreferences = getSharedPreferences("UserData", MODE_PRIVATE);
                SharedPreferences.Editor editor = sharedPreferences.edit();
                editor.putString("ChildName", editTextChildName.getText().toString());
                editor.putString("ChildAddress", editTextChildAddress.getText().toString());
                editor.putString("LearningWorkplace", editTextLearningWorkplace.getText().toString());
                editor.putString("ChildContact", editTextChildContact.getText().toString());
                editor.apply();

                startActivity(intent);
            }
        });
    }
}
