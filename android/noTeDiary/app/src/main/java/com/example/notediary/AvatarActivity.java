package com.example.notediary;

import android.content.Intent;
import android.content.SharedPreferences;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;

import java.io.IOException;

public class AvatarActivity extends AppCompatActivity {
    private static final int PICK_IMAGE_REQUEST = 1;
    private Uri imageUri;
    private ImageView imageViewAvatar;
    private Button buttonChooseAvatar;
    private Button buttonNextToEmail;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_avatar);

        imageViewAvatar = findViewById(R.id.imageViewAvatar);
        buttonChooseAvatar = findViewById(R.id.buttonChooseAvatar);
        buttonNextToEmail = findViewById(R.id.buttonNextToEmail);

        // Изначально делаем кнопку визуально "неактивной" (серой)
        buttonNextToEmail.setAlpha(0.5f);
        // Можно установить начальный серый фон
        buttonNextToEmail.setBackgroundColor(Color.GRAY);

        // Устанавливаем обрезку изображения по контуру
        imageViewAvatar.setClipToOutline(true);

        buttonChooseAvatar.setOnClickListener(v -> openFileChooser());

        // Обработчик нажатия кнопки "Далее"
        buttonNextToEmail.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (imageUri != null) {
                    // Если изображение загружено, переходим к следующей активности
                    Intent intent = new Intent(AvatarActivity.this, ConsentActivity.class);
                    SharedPreferences sharedPreferences = getSharedPreferences("UserData", MODE_PRIVATE);
                    SharedPreferences.Editor editor = sharedPreferences.edit();
                    editor.putString("AvatarUri", imageUri.toString());
                    editor.apply();
                    startActivity(intent);
                } else {
                    // Если изображение не загружено, выводим сообщение
                    Toast.makeText(AvatarActivity.this, "Нужно загрузить фотографию!", Toast.LENGTH_SHORT).show();
                }
            }
        });
    }

    // Открытие выбора файла
    private void openFileChooser() {
        Intent intent = new Intent(Intent.ACTION_PICK, MediaStore.Images.Media.EXTERNAL_CONTENT_URI);
        intent.setType("image/*");
        startActivityForResult(intent, PICK_IMAGE_REQUEST);
    }

    // Обработка результата выбора файла
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode == PICK_IMAGE_REQUEST && resultCode == RESULT_OK && data != null && data.getData() != null) {
            imageUri = data.getData();
            try {
                Bitmap bitmap = MediaStore.Images.Media.getBitmap(getContentResolver(), imageUri);
                imageViewAvatar.setImageBitmap(bitmap);  // Устанавливаем изображение на ImageView

                // После загрузки изображения делаем кнопку визуально активной и меняем её фон на зелёный
                buttonNextToEmail.setAlpha(1.0f);
                buttonNextToEmail.setBackgroundColor(getResources().getColor(R.color.green));
            } catch (IOException e) {
                Toast.makeText(this, "Ошибка загрузки изображения", Toast.LENGTH_SHORT).show();
            }
        }
    }
}
