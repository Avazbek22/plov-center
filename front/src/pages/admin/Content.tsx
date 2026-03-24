import { useEffect } from 'react'
import { useForm, Controller } from 'react-hook-form'
import Typography from '@mui/material/Typography'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import TextField from '@mui/material/TextField'
import Button from '@mui/material/Button'
import CircularProgress from '@mui/material/CircularProgress'
import Box from '@mui/material/Box'
import Stack from '@mui/material/Stack'
import { useContentQuery, useUpdateAbout, useUpdateContacts } from '@/hooks/use-content'
import ImageUpload from '@/components/shared/ImageUpload'
import type { AboutFormData, ContactsFormData } from '@/types/content'

export default function Content() {
  const { data, isLoading } = useContentQuery()

  const aboutForm = useForm<AboutFormData>({
    defaultValues: { text: '', photoPath: '' },
  })

  const contactsForm = useForm<ContactsFormData>({
    defaultValues: { address: '', phone: '', hours: '', mapEmbed: '' },
  })

  const updateAbout = useUpdateAbout()
  const updateContacts = useUpdateContacts()

  useEffect(() => {
    if (!data) return
    aboutForm.reset({
      text: data.about.text ?? '',
      photoPath: data.about.photoPath ?? '',
    })
  }, [data, aboutForm])

  useEffect(() => {
    if (!data) return
    contactsForm.reset({
      address: data.contacts.address ?? '',
      phone: data.contacts.phone ?? '',
      hours: data.contacts.hours ?? '',
      mapEmbed: data.contacts.mapEmbed ?? '',
    })
  }, [data, contactsForm])

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 6 }}>
        <CircularProgress />
      </Box>
    )
  }

  return (
    <Box>
      <Typography variant="h4" sx={{ mb: 3 }}>
        Контент сайта
      </Typography>

      <Stack spacing={3}>
        <Card>
          <CardContent>
            <Typography variant="h6" sx={{ mb: 2 }}>
              О нас
            </Typography>

            <Box
              component="form"
              onSubmit={aboutForm.handleSubmit((values) => updateAbout.mutate(values))}
            >
              <Stack spacing={2}>
                <Controller
                  control={aboutForm.control}
                  name="text"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="Текст"
                      multiline
                      rows={4}
                      fullWidth
                    />
                  )}
                />

                <Controller
                  control={aboutForm.control}
                  name="photoPath"
                  render={({ field }) => (
                    <ImageUpload
                      value={field.value || null}
                      onChange={(path) => field.onChange(path ?? '')}
                      area="about"
                    />
                  )}
                />

                <Box>
                  <Button
                    type="submit"
                    variant="contained"
                    disabled={updateAbout.isPending}
                  >
                    Сохранить
                  </Button>
                </Box>
              </Stack>
            </Box>
          </CardContent>
        </Card>

        <Card>
          <CardContent>
            <Typography variant="h6" sx={{ mb: 2 }}>
              Контакты
            </Typography>

            <Box
              component="form"
              onSubmit={contactsForm.handleSubmit((values) => updateContacts.mutate(values))}
            >
              <Stack spacing={2}>
                <Controller
                  control={contactsForm.control}
                  name="address"
                  render={({ field }) => (
                    <TextField {...field} label="Адрес" fullWidth />
                  )}
                />

                <Controller
                  control={contactsForm.control}
                  name="phone"
                  render={({ field }) => (
                    <TextField {...field} label="Телефон" fullWidth />
                  )}
                />

                <Controller
                  control={contactsForm.control}
                  name="hours"
                  render={({ field }) => (
                    <TextField {...field} label="Часы работы" fullWidth />
                  )}
                />

                <Controller
                  control={contactsForm.control}
                  name="mapEmbed"
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="Код карты (HTML embed)"
                      multiline
                      rows={3}
                      fullWidth
                    />
                  )}
                />

                <Box>
                  <Button
                    type="submit"
                    variant="contained"
                    disabled={updateContacts.isPending}
                  >
                    Сохранить
                  </Button>
                </Box>
              </Stack>
            </Box>
          </CardContent>
        </Card>
      </Stack>
    </Box>
  )
}
